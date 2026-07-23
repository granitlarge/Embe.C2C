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
}