using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IMatchingRepository : IGenericRepository<Matching, Guid>
{
    public Task<List<Matching>> GetMatchingsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
    public Task<Matching?> GetMatchingByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<List<Matching>> GetByUserIdAsync
    (
        Guid userId,
        CancellationToken cancellationToken
    );

    public Task<Matching?> GetByMessageIdAsync(Guid messageId, CancellationToken cancellationToken);

    Task<IsParticipantInMatchingFact> GetIsParticipantInMatchingFactAsync(Guid currentUserId, Guid matchingId, CancellationToken cancellationToken);
}