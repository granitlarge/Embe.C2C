using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.Candidates.Facts;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class CandidateRepository(C2CContext context) : ICandidateRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<Candidate> Set => new MyDbSet<Candidate>(_context.Candidates);

    public Task<Candidate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Candidates.SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);
    }

    public Task<Candidate?> GetByParametersAsync(Guid userId, Guid candidateUserId, Guid userSearchProfileId, Guid candidateSearchProfileId, CancellationToken cancellationToken)
    {
        return _context.Candidates.SingleOrDefaultAsync
        (c =>
            c.UserId == userId &&
            c.CandidateUserId == candidateUserId &&
            c.UserSearchProfileId == userSearchProfileId &&
            c.CandidateSearchProfileId == candidateSearchProfileId,
            cancellationToken
        );
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AuthorizationFact>> GetAuthorizationFactsAsync
    (
        Guid currentUserId,
        Guid candidateId,
        CancellationToken cancellationToken
    )
    {
        var facts = await _context.Candidates
            .Select(c => new
            {
                c.Id,
                IsOwner = c.UserId == currentUserId,
                IsCandidate = c.CandidateUserId == currentUserId,
                IsPositivelyJudgedCandidate = c.CandidateUserId == currentUserId && c.Judgement == true
            })
            .SingleOrDefaultAsync(c => c.Id == candidateId, cancellationToken);

        return
        [
            new IsOwner(candidateId, facts?.IsOwner ?? false),
            new IsCandidate(candidateId, facts?.IsCandidate ?? false),
            new IsPositivelyJudgedCandidate(candidateId, facts?.IsPositivelyJudgedCandidate ?? false)
        ];

    }

    public Task<List<Candidate>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _context
            .Candidates
            .Where(c => c.UserId == userId)
            .Include(c => c.CandidateUser)
            .Include(c => c.CandidateSearchProfile)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Candidate>> GetPositiveJudgementsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        return _context
            .Candidates
            .AsSplitQuery()
            .Where(c => c.CandidateUserId == userId)
            .Include(c => c.User)
            .Include(c => c.UserSearchProfile)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> GenerateCandidatesForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingCandidates = await _context.Candidates.AnyAsync(c => c.UserId == userId && c.Judgement == null, cancellationToken);
        if (existingCandidates)
        {
            return true;
        }

#warning This query needs to order results by relevance, but for now it just returns the first 20 candidates that match the criteria. The ordering can be improved later.

        var candidateIds = await _context.Database.SqlQueryRaw<CandidateIds>($"""

        select c."Id" CandidateId, usp."Id" UserSearchProfileId, csp."Id" CandidateSearchProfileId
        from "DomainUsers" u
        inner join "SearchProfiles" usp on u."Id" = usp."UserId"
        inner join "DomainUsers" c on (ST_Distance(u."Coordinates", c."Coordinates") <= usp."MaximumDistance" * 1000 or usp."MaximumDistance" is null)
        inner join "SearchProfiles" csp on csp."UserId" = c."Id" and (ST_Distance(u."Coordinates", c."Coordinates") <= csp."MaximumDistance" * 1000 or csp."MaximumDistance" is null)
        where 1=1
        and u."Id" = '{userId}'
        and c."Id" != u."Id"
        and usp."Active" = true
        and csp."Active" = true
        and usp."RelationshipType" = csp."RelationshipType"
        and exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = usp."Id" and spg."Gender" = c."Gender")
        and exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = csp."Id" and spg."Gender" = u."Gender")
        and extract(year from age(CURRENT_DATE, u."BirthDate")) between coalesce(csp."AgeRangeMin", 18) and coalesce(csp."AgeRangeMax", 120)
        and extract(year from age(CURRENT_DATE, c."BirthDate")) between coalesce(usp."AgeRangeMin", 18) and coalesce(usp."AgeRangeMax", 120)
        and (
            -- we're adding some fuzziness to the search
            -- if the desired frequency differs from "once" accept a mismatch of 1 step, so those who seek daily will be able to see those who seek weekly,
            -- those who seek weekly will be able to see those who seek monthly, and vice versa.
            (usp."Engagement_Frequency" = 0 or csp."Engagement_Frequency" = 0) and usp."Engagement_Frequency" = csp."Engagement_Frequency"
            or abs(usp."Engagement_Frequency" - csp."Engagement_Frequency") <= 1
        )
        and usp."Engagement_Boundedness" = csp."Engagement_Boundedness"
        and (
            -- fuzzying here as well
            -- if someone is searching for "hybrid" and someone else is searching for "virtual", we'll show em to each other despite there
            -- not being a perfect match, one of them could compromise 	
            usp."Engagement_Medium" = csp."Engagement_Medium"
            or usp."Engagement_Medium" = 2
            or csp."Engagement_Medium" = 2
        )
        and (
            usp."Engagement_Boundedness" != 2 
            or daterange(usp."Engagement_StartDate", usp."Engagement_EndDate") && daterange(csp."Engagement_StartDate", csp."Engagement_EndDate")
        )
        and not exists (select * from "Blockings" b where b."BlockerUserId" = u."Id" and b."BlockedUserId" = c."Id")
        and not exists (select * from "Blockings" b where b."BlockerUserId" = c."Id" and b."BlockedUserId" = u."Id")
        and not exists (select * from "Matchings" m where m."UserId1" = u."Id" and m."UserId2" = c."Id")
        and not exists (select * from "Matchings" m where m."UserId1" = c."Id" and m."UserId2" = u."Id")
        and not exists (select * 
                        from "Candidates" can
                        where can."UserId" = c."Id" and can."CandidateUserId" = u."Id" and can."Judgement" = false)
        and not exists (select * 
                        from "Candidates" can 
                        where can."UserId" = u."Id" 
                        and can."CandidateUserId" = c."Id"
                        and can."UserSearchProfileId" = usp."Id" 
                        and can."CandidateSearchProfileId" = csp."Id")
        offset 0 
        limit 20

        """).ToListAsync(cancellationToken);

        var candidates1 = candidateIds.Select(c => Candidate.Create(userId, c.CandidateId, c.UserSearchProfileId, c.CandidateSearchProfileId)).ToList();
        var candidates2 = candidateIds.Select(c => Candidate.Create(c.CandidateId, userId, c.CandidateSearchProfileId, c.UserSearchProfileId)).ToList();
        _context.Candidates.AddRange(candidates1);
        _context.Candidates.AddRange(candidates2);

        await SaveChangesAsync(cancellationToken);

        return candidates1.Count > 0;

    }

    public record CandidateIds
    (
        Guid CandidateId,
        Guid UserSearchProfileId,
        Guid CandidateSearchProfileId
    );
}