using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Domain.Aggregates.Messages;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IMessageRepository : IGenericRepository<Message, Guid>
{
    public Task<List<Message>> GetMessagesByMatchingIdAsync
    (
        Guid matchingId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );

    public Task<AuthorizationFact[]> GetAuthorizationFactsAsync(Guid currentUserId, Guid messageId, CancellationToken cancellationToken);
    public Task<List<Message>> GetMessagesByMessageIdsAsync(ImmutableHashSet<Guid> messageId, CancellationToken cancellationToken);
    public Task<Message?> GetMessageByIdIncludeReplyAsync(Guid messageId, CancellationToken cancellationToken);
    public Task<Message?> GetLastMessageAsync(Guid matchingId, Guid exceptMessageId, CancellationToken cancellationToken);
    public Task<List<Message>> GetRepliesAsync(Guid messageId, CancellationToken cancellationToken);
}