using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class DeleteMessageHandler : CommandHandler<DeleteMessageCommand, Result>
{
    private readonly MessageAuthorizationService _messageAuthorizationPolicy;
    private readonly IAuthenticatedUserService _authenticatedUser;
    private readonly MatchingService _matchingService;

    public DeleteMessageHandler
    (
        MessageAuthorizationService messageAuthorizationPolicy,
        IAuthenticatedUserService authenticatedUser,
        MatchingService matchingService,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _messageAuthorizationPolicy = messageAuthorizationPolicy;
        _authenticatedUser = authenticatedUser;
        _matchingService = matchingService;
    }

    protected async override Task<CommandResult<Result>> HandleAsync
    (
        ISparseRepository context,
        DeleteMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _messageAuthorizationPolicy.GetPermissionsAsync(command.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.Delete))
        {
            return new CommandResult<Result>(false, Result.Failure(FailureReason.Forbidden, "You don't have permission to delete this message."));
        }

        try
        {
            var user = await context.DomainUsersQuery.SingleOrDefaultAsync(u => u.Id == _authenticatedUser.UserId, cancellationToken);
            if (user is null)
                return new CommandResult<Result>(false, Result.Failure(FailureReason.Forbidden, "Authenticated user not found."));

            var message = await context.MessagesQuery.SingleOrDefaultAsync(m => m.Id == command.MessageId, cancellationToken);
            if (message is null)
                return new CommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "Message not found."));

            var matching = await context.MatchingsQuery.SingleOrDefaultAsync(m => m.Messages!.Any(msg => msg.Id == message.Id), cancellationToken);
            if (matching is null)
                return new CommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "Matching not found for the message."));

            var newLastMessage = await context.MessagesQuery
                .Where(m => m.MatchingId == matching.Id && m.Id != message.Id)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var replies = await context.MessagesQuery
                .Where(m => m.ReplyToMessageId == message.Id)
                .ToListAsync(cancellationToken);

            _matchingService.DeleteMessage(user, message, newLastMessage, matching, replies);
            context.Messages.Remove(message);

            var result = Result.Success();
            return new CommandResult<Result>(true, result);
        }
        catch (DomainException ex)
        {
            var result = Result.Failure(FailureReason.DomainError, ex.Message);
            var transactionalResult = new CommandResult<Result>(false, result);
            return transactionalResult;
        }
    }
}