using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Domain.Aggregates.Candidates;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface ICandidateRepository : IGenericRepository<Candidate, Guid>
{
    public Task<Candidate?> GetByParametersAsync
    (
        Guid userId,
        Guid candidateUserId,
        Guid userSearchProfileId,
        Guid candidateSearchProfileId,
        CancellationToken cancellationToken
    );

    public Task<List<AuthorizationFact>> GetAuthorizationFactsAsync(Guid currentUserId, Guid candidateId, CancellationToken cancellationToken);
    public Task<List<Candidate>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    public Task<List<Candidate>> GetPositiveJudgementsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
    public Task<bool> GenerateCandidatesForUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}