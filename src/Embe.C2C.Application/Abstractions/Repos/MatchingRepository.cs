using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IMatchingRepository : IAggregateRepository<Matching, Guid>
{
    public Task<List<Matching>> GetByUserIdAsync
    (
        Guid userId,
        bool includeUser1,
        bool includeUser2,
        bool includeUser1SearchProfile,
        bool includeUser2SearchProfile,
        bool includeLastMessage,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );

    public Task<Matching?> GetByIdAsync
    (
        Guid id,
        bool includeUser1,
        bool includeUser2,
        bool includeUser1SearchProfile,
        bool includeUser2SearchProfile,
        bool includeLastMessage,
        bool includeMessages,
        bool includeMessagesReplyToMessage,
        int numberOfMessagesToInclude,
        CancellationToken cancellationToken
    );

    public Task<Matching?> GetByMessageIdAsync(Guid messageId, CancellationToken cancellationToken);

    Task<IsParticipantInMatchingFact> GetIsParticipantInMatchingFactAsync(Guid currentUserId, Guid matchingId, CancellationToken cancellationToken);
}