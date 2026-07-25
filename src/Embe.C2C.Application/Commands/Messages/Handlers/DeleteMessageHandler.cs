using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class DeleteMessageHandler
(
    IMessageRepository messageRepo,
    IMatchingRepository matchingRepo,
    IUserRepository userRepo,
    MessageAuthorizationService messageAuthorizationPolicy,
    IAuthenticatedUserService authenticatedUser,
    MatchingService matchingService,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    DomainEventStore domainEventStore
) : CommandHandler<DeleteMessageCommand, ErrorOr<Success>>(domainEventStore, context, domainEventHandler, integrationEventHandler)
{
    private readonly IMessageRepository _messageRepo = messageRepo;
    private readonly IMatchingRepository _matchingRepo = matchingRepo;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly MessageAuthorizationService _messageAuthorizationPolicy = messageAuthorizationPolicy;
    private readonly IAuthenticatedUserService _authenticatedUser = authenticatedUser;
    private readonly MatchingService _matchingService = matchingService;

    protected async override Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        DeleteMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _messageAuthorizationPolicy.GetPermissionsAsync(command.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.Delete))
        {
            return new(false, ApplicationErrors.Forbidden.ToForbiddenErrorOr());
        }

        var user = await _userRepo.GetByIdAsync(_authenticatedUser.UserId ?? throw new InvalidOperationException("user is not authenticated"), cancellationToken);
        if (user is null)
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());

        var message = await _messageRepo.GetByIdAsync(command.MessageId, cancellationToken);
        if (message is null)
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());

        var matching = await _matchingRepo.GetByMessageIdAsync(message.Id, cancellationToken);
        if (matching is null)
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());

        var newLastMessage = await _messageRepo.GetLastMessageAsync(matching.Id, message.Id, cancellationToken);
        var replies = await _messageRepo.GetRepliesAsync(message.Id, cancellationToken);

        _matchingService.DeleteMessage(user, message, newLastMessage, matching, replies);
        _messageRepo.Set.Remove(message);

        return new(true, Result.Success);
    }
}