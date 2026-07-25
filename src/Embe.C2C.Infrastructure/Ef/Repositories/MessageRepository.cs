using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.Messages.Facts;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class MessageRepository
(
    C2CContext context
) : IMessageRepository
{

    private readonly C2CContext _context = context;

    public IDbSet<Message> Set => new MyDbSet<Message>(_context.Messages);

    public async Task<AuthorizationFact[]> GetAuthorizationFactsAsync
    (
        Guid currentUserId,
        Guid messageId,
        CancellationToken cancellationToken
    )
    {
        var facts = await _context
            .Messages
            .Where(m => m.Id == messageId)
            .Select(m => new
            {
                m.AuthorUserId,
                RecipientUserId = m.Matching!.UserId1 != m.AuthorUserId ? m.Matching.UserId1 : m.Matching.UserId2
            })
            .SingleOrDefaultAsync(cancellationToken);

        AuthorMessageFact authorMessageFact;
        RecipientMessageFact recipientMessageFact;
        if (facts is null)
        {
            authorMessageFact = new AuthorMessageFact(messageId, false);
            recipientMessageFact = new RecipientMessageFact(messageId, false);
        }
        else
        {
            authorMessageFact = new AuthorMessageFact(messageId, facts.AuthorUserId == currentUserId);
            recipientMessageFact = new RecipientMessageFact(messageId, facts.RecipientUserId == currentUserId);
        }

        return [authorMessageFact, recipientMessageFact];
    }

    public Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Messages.SingleOrDefaultAsync(message => message.Id == id, cancellationToken);
    }

    public Task<Message?> GetMessageByIdIncludeReplyAsync(Guid messageId, CancellationToken cancellationToken)
    {
        return _context.Messages.Include(m => m.ReplyToMessage).SingleOrDefaultAsync(m => m.Id == messageId, cancellationToken);
    }

    public Task<List<Message>> GetMessagesByMatchingIdAsync
    (
        Guid matchingId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        return _context.Messages
            .Where(m => m.MatchingId == matchingId)
                .Include(m => m.ReplyToMessage)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Message>> GetMessagesByMessageIdsAsync
    (
        ImmutableHashSet<Guid> messageId,
        CancellationToken cancellationToken
    )
    {
        return _context.Messages
            .Where(m => messageId.Contains(m.Id))
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<Message?> GetLastMessageAsync(Guid matchingId, Guid exceptMessageId, CancellationToken cancellationToken)
    {
        return _context.Messages
            .Where(m => m.MatchingId == matchingId && m.Id != exceptMessageId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Message>> GetRepliesAsync(Guid messageId, CancellationToken cancellationToken)
    {
        return _context.Messages.Where(m => m.ReplyToMessageId == messageId).ToListAsync(cancellationToken);
    }
}