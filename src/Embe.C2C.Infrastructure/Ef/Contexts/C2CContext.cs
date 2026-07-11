using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Entities.SearchProfiles;
using Embe.C2C.Infrastructure.Ef.Entities;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Infrastructure.Ef.Contexts;

public class MyDbSet<T>(DbSet<T> dbSet) : IDbSet<T> where T : class
{
    private readonly DbSet<T> _dbSet = dbSet;

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public ValueTask<T?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync(keyValues, cancellationToken);
    }
}

public class C2CContext
(
    DbContextOptions<C2CContext> options
) : IdentityDbContext<MyIdentityUser>(options), IRepository
{
    public DbSet<User> DomainUsers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Judgement> Judgements { get; set; }
    public DbSet<Matching> Matchings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Blocking> Blockings { get; set; }
    public DbSet<SearchProfile> SearchProfiles { get; set; }
    public DbSet<AdminArea> AdminAreas { get; set; }
    public DbSet<Candidate> Candidates { get; set; }

    public IImmutableList<DomainEvent> DomainEvents
    {
        get
        {
            return ChangeTracker.Entries()
                .Select(e => e.Entity)
                .OfType<DomainEventCollector>()
                .SelectMany(c => c.DomainEvents)
                .ToImmutableList();
        }
    }

    public IQueryable<User> DomainUsersQuery
    {
        get
        {
            return DomainUsers;
        }
    }

    public IQueryable<Account> AccountsQuery
    {
        get
        {
            return Accounts;
        }
    }

    public IQueryable<Judgement> JudgementsQuery
    {
        get
        {
            return Judgements;
        }
    }

    public IQueryable<Matching> MatchingsQuery
    {
        get
        {
            return Matchings.Include(m => m.Conversation);
        }
    }

    public IQueryable<Notification> NotificationsQuery
    {
        get
        {
            return Notifications;
        }
    }

    IDbSet<User> ISparseRepository.DomainUsers
    {
        get
        {
            return new MyDbSet<User>(DomainUsers);
        }
    }

    IDbSet<Account> ISparseRepository.Accounts
    {
        get
        {
            return new MyDbSet<Account>(Accounts);
        }
    }

    IDbSet<Judgement> ISparseRepository.Judgements
    {
        get
        {
            return new MyDbSet<Judgement>(Judgements);
        }
    }

    IDbSet<Matching> ISparseRepository.Matchings
    {
        get
        {
            return new MyDbSet<Matching>(Matchings);
        }
    }

    IDbSet<Notification> ISparseRepository.Notifications
    {
        get
        {
            return new MyDbSet<Notification>(Notifications);
        }
    }

    public IQueryable<Message> MessagesQuery => Messages;

    IDbSet<Message> ISparseRepository.Messages => new MyDbSet<Message>(Messages);

    IDbSet<Blocking> ISparseRepository.Blockings => new MyDbSet<Blocking>(Blockings);

    public IQueryable<Blocking> BlockingsQuery => Blockings;

    IDbSet<SearchProfile> ISparseRepository.SearchProfiles => new MyDbSet<SearchProfile>(SearchProfiles);

    public IQueryable<SearchProfile> SearchProfilesQuery => SearchProfiles;

    public IQueryable<IAdminArea> AdminAreasQuery => AdminAreas;

    public IQueryable<Candidate> CandidatesQuery => Candidates;

    IDbSet<Candidate> ISparseRepository.Candidates => new MyDbSet<Candidate>(Candidates);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(System.Data.IsolationLevel.Snapshot, cancellationToken);
    }

    public record CandidateIds
    (
        Guid CandidateId,
        Guid UserSearchProfileId,
        Guid CandidateSearchProfileId
    );
    public async Task<bool> GenerateCandidatesForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingCandidates = await Candidates.AnyAsync(c => c.UserId == userId && c.Judgement == null, cancellationToken);
        if (existingCandidates)
        {
            return true;
        }

#warning This query needs to order results by relevance, but for now it just returns the first 20 candidates that match the criteria. The ordering can be improved later.

        var candidateIds = await Database.SqlQueryRaw<CandidateIds>($"""

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
        and (
            exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = usp."Id" and spg."Gender" = c."Gender")
        )
        and (
            exists (select * from "SearchProfileGender" spg where spg."SearchProfileId" = csp."Id" and spg."Gender" = u."Gender")
        )
        and extract(year from age(CURRENT_DATE, u."BirthDate")) between coalesce(csp."AgeRangeMin", 0) and coalesce(csp."AgeRangeMax", 120)
        and extract(year from age(CURRENT_DATE, c."BirthDate")) between coalesce(usp."AgeRangeMin", 0) and coalesce(usp."AgeRangeMax", 120)
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
                        from "Judgements" j 
                        inner join "Candidates" can on can."Id" = j."CandidateId"
                        where can."UserId" = u."Id" and can."CandidateUserId" = c."Id")
        and not exists (select * 
                        from "Judgements" j 
                        inner join "Candidates" can on can."Id" = j."CandidateId"
                        where can."UserId" = c."Id" and can."CandidateUserId" = u."Id" and j."IsPositive" = false)
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
        Candidates.AddRange(candidates1);
        Candidates.AddRange(candidates2);

        await SaveChangesAsync(cancellationToken);

        return candidates1.Count > 0;
    }

    public async Task<List<IAdminArea>> SearchAdminAreasAsync
    (
        string? parentId, 
        double? longitude, 
        double? latitude, 
        int page,
        int size,
        CancellationToken cancellationToken = default
    )
    {
        var maxDistanceMeters = 1000;
        List<AdminArea> result = [];
        while (result.Count == 0  && maxDistanceMeters <= 25_000_000)
        {
            var pointFilter = (longitude.HasValue && latitude.HasValue) ? new NetTopologySuite.Geometries.Point(longitude.Value, latitude.Value) { SRID = 4326 } : null;
            var parentIdFilter = !string.IsNullOrEmpty(parentId) ? parentId : null;
            result = await AdminAreas
            .AsNoTracking()
            .Where(aa => pointFilter == null || aa.Point != null && aa.Point.Distance(pointFilter) <= maxDistanceMeters)
            .Where(aa => parentIdFilter == null || aa.ParentId == parentIdFilter)
            .OrderBy(aa => pointFilter != null && aa.Point != null ? aa.Point.Distance(pointFilter) : int.MaxValue)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
            maxDistanceMeters *= 2;
        }

        return [.. result.Cast<IAdminArea>()];
    }

    public async Task<List<IAdminArea>> ReverseGeocodeAsync(double longitude, double latitude)
    {
        var adminArea = (await SearchAdminAreasAsync(null, longitude, latitude, 1, 1)).FirstOrDefault();
        if (adminArea == null)
        {
            return [];
        }

        var adminAreas = new List<IAdminArea> { adminArea };
        var highestLevelAdminArea = adminAreas[0];
        while (highestLevelAdminArea.ParentId != null)
        {
            var parent = await AdminAreas.AsNoTracking().FirstOrDefaultAsync(aa => aa.Id == highestLevelAdminArea.ParentId);
            if (parent == null)
            {
                break;
            }
            adminAreas.Add(parent);
            highestLevelAdminArea = parent;
        }

        return adminAreas;
    }

    public Task<bool> IsCandidateSearchProfileForUserIdAsync(Guid userId, Guid searchProfileId, CancellationToken cancellationToken = default)
    {
        return Candidates.AnyAsync(c => c.UserId == userId && c.CandidateSearchProfileId == searchProfileId, cancellationToken);
    }
}