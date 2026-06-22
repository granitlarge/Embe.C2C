using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class EditMessageHandler : TransactionalCommandHandler<EditMessageCommand, Result<ReadDto<MessageDto, MessagePermission>>>
{
    private readonly MessageAuthorizationPolicy _authorizationPolicy;
    private readonly IAuthenticatedUserService _authenticatedUser;
    private readonly MatchingService _matchingService;

    public EditMessageHandler
    (
        MessageAuthorizationPolicy authorizationPolicy,
        IAuthenticatedUserService authenticatedUser,
        MatchingService matchingService,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authorizationPolicy = authorizationPolicy;
        _authenticatedUser = authenticatedUser;
        _matchingService = matchingService;
    }

    protected async override Task<TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>> HandleAsync
    (
        ISparseRepository context,
        EditMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(command.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.Edit))
        {
            var result = Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "You don't have permission to edit this message.");
            return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, result);
        }

        try
        {
            var user = await context.DomainUsersQuery.SingleOrDefaultAsync(u => u.Id == _authenticatedUser.UserId, cancellationToken);
            if (user is null)
                return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "Authenticated user not found."));

            var message = await context.MessagesQuery.SingleOrDefaultAsync(m => m.Id == command.MessageId, cancellationToken);
            if (message is null)
                return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Message not found."));

            _matchingService.EditMessage(user, message, MessageContent.Create(command.NewContent));

            var dto = await _authorizationPolicy.ToDtoAsync(message, cancellationToken) ?? throw new InvalidOperationException("A message was edited but failed to convert to DTO.");
            return new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(true, Result<ReadDto<MessageDto, MessagePermission>>.Success(dto));
        }
        catch (DomainException ex)
        {
            var result = Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.DomainError, ex.Message);
            var transactionalResult = new TransactionalCommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, result);
            return transactionalResult;
        }
    }
}