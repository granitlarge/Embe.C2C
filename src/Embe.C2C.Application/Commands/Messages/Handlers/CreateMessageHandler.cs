using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Policies;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class CreateMessageHandler : TransactionalCommandHandler<CreateMessageCommand, Result<ReadDto<MessageDto, MessagePermission>>>
{
    private readonly MatchingAuthorizationPolicy _matchingAuthorizationPolicy;
    private readonly MessageAuthorizationPolicy _messageAuthorizationPolicy;
    private readonly IAuthenticatedUserService _authenticatedUser;
    private readonly MatchingService _matchingService;

    public CreateMessageHandler
    (
        MatchingAuthorizationPolicy matchingAuthorizationPolicy,
        MessageAuthorizationPolicy messageAuthorizationPolicy,
        IAuthenticatedUserService authenticatedUser,
        MatchingService matchingService,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _matchingAuthorizationPolicy = matchingAuthorizationPolicy;
        _messageAuthorizationPolicy = messageAuthorizationPolicy;
        _authenticatedUser = authenticatedUser;
        _matchingService = matchingService;
    }

    protected async override Task<TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>> HandleAsync
    (
        ISparseRepository context,
        CreateMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _matchingAuthorizationPolicy.GetPermissionsAsync(command.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.Chat))
        {
            return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "You don't have permission to send messages in this matching."));
        }

        try
        {
            var messageContent = MessageContent.Create(command.Content);
            var user = await context.DomainUsersQuery.SingleAsync(u => u.Id == _authenticatedUser.UserId, cancellationToken);
            if (user is null)
                return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "Authenticated user not found."));
            var matching = await context.MatchingsQuery.SingleAsync(m => m.Id == command.MatchingId, cancellationToken);
            if (matching is null)
                return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Matching not found."));
            var receiver = await context.DomainUsersQuery.SingleAsync(u => u.Id == matching.GetOtherUserId(_authenticatedUser.UserId), cancellationToken);
            if (receiver is null)
                return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Receiver user not found."));

            var blocking1 = await context.BlockingsQuery.FirstOrDefaultAsync(b => b.BlockerUserId == user.Id && b.BlockedUserId == receiver.Id, cancellationToken);
            var blocking2 = await context.BlockingsQuery.FirstOrDefaultAsync(b => b.BlockerUserId == receiver.Id && b.BlockedUserId == user.Id, cancellationToken);

            var communicationPolicy = new CommunicationPolicy(user, receiver, matching, blocking1, blocking2);
            var message = _matchingService.SendMessage(user, matching, messageContent, communicationPolicy);
            context.Messages.Add(message);
            var dto = await _messageAuthorizationPolicy.ToDtoAsync(message, cancellationToken) ??
                throw new InvalidOperationException("The message should be viewable to the sender right after sending it.");
            var result = Result<ReadDto<MessageDto, MessagePermission>>.Success(dto);
            return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(true, result);
        }
        catch (DomainException ex)
        {
            var result = Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.DomainError, ex.Message);
            var transactionalResult = new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, result);
            return transactionalResult;
        }
    }
}