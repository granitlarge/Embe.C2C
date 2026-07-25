using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Services;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class DeleteMessageHandler : CommandHandler<DeleteMessageCommand, ErrorOr<Success>>
{
    private readonly IMessageRepository _messageRepo;
    private readonly IMatchingRepository _matchingRepo;
    private readonly IUserRepository _userRepo;
    private readonly MessageAuthorizationService _messageAuthorizationPolicy;
    private readonly IAuthenticatedUserService _authenticatedUser;
    private readonly MatchingService _matchingService;

    public DeleteMessageHandler
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
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _matchingRepo = matchingRepo;
        _userRepo = userRepo;
        _messageAuthorizationPolicy = messageAuthorizationPolicy;
        _authenticatedUser = authenticatedUser;
        _matchingService = matchingService;
        _messageRepo = messageRepo;
    }

    protected async override Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        DeleteMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _messageAuthorizationPolicy.GetPermissionsAsync(command.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.Delete))
        {
            return new CommandResult<ErrorOr<Success>>(false, Error.Forbidden("forbidden", "Authenticated user does not have permission to delete this message."));
        }

        var user = await _userRepo.GetByIdAsync(_authenticatedUser.UserId ?? throw new InvalidOperationException("user is not authenticated"), cancellationToken);
        if (user is null)
            return new CommandResult<ErrorOr<Success>>(false, Error.NotFound("user_not_found", "Authenticated user not found."));

        var message = await _messageRepo.GetByIdAsync(command.MessageId, cancellationToken);
        if (message is null)
            return new CommandResult<ErrorOr<Success>>(false, Error.NotFound("message_not_found", "Message not found."));

        var matching = await _matchingRepo.GetByMessageIdAsync(message.Id, cancellationToken);
        if (matching is null)
            return new CommandResult<ErrorOr<Success>>(false, Error.NotFound("matching_not_found", "Matching not found for the message."));

        var newLastMessage = await _messageRepo.GetLastMessageAsync(matching.Id, message.Id, cancellationToken);
        var replies = await _messageRepo.GetRepliesAsync(message.Id, cancellationToken);

        _matchingService.DeleteMessage(user, message, newLastMessage, matching, replies);
        _messageRepo.Set.Remove(message);

        return new CommandResult<ErrorOr<Success>>(true, Result.Success);
    }
}