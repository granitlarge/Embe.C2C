using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class EditMessageHandler : CommandHandler<EditMessageCommand, Result<ReadDto<MessageDto, MessagePermission>>>
{
    private readonly IMessageRepository _messageRepo;
    private readonly IUserRepository _userRepo;
    private readonly MessageAuthorizationService _messageAuthorizationService;
    private readonly IAuthenticatedUserService _authenticatedUser;
    private readonly MatchingService _matchingService;
    private readonly MessageDtoMapper _messageDtoMapper;

    public EditMessageHandler
    (
        IMessageRepository messageRepo,
        IUserRepository userRepo,
        MessageAuthorizationService messageAuthorizationService,
        IAuthenticatedUserService authenticatedUser,
        MatchingService matchingService,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        DomainEventStore domainEventStore,
        MessageDtoMapper messageDtoMapper
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _userRepo = userRepo;
        _messageAuthorizationService = messageAuthorizationService;
        _authenticatedUser = authenticatedUser;
        _matchingService = matchingService;
        _messageDtoMapper = messageDtoMapper;
        _messageRepo = messageRepo;
    }

    protected async override Task<CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>> InternalHandleAsync
    (
        EditMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _messageAuthorizationService.GetPermissionsAsync(command.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.Edit))
        {
            var result = Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "You don't have permission to edit this message.");
            return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, result);
        }

        try
        {
            var user = await _userRepo.GetByIdAsync(_authenticatedUser.UserId ?? throw new InvalidOperationException("user is not authenticated"), cancellationToken);
            if (user is null)
                return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "Authenticated user not found."));

            var message = await _messageRepo.GetMessageByIdIncludeReplyAsync(command.MessageId, cancellationToken);
            if (message is null)
                return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Message not found."));

            _matchingService.EditMessage(user, message, MessageContent.Create(command.NewContent));

            var readDto = await message.ToDtoAsync(_messageAuthorizationService, _messageDtoMapper, cancellationToken);
            if (readDto == null)
            {
                return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "You don't have permission to view this message."));
            }

            return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(true, Result<ReadDto<MessageDto, MessagePermission>>.Success(readDto));
        }
        catch (DomainException ex)
        {
            var result = Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.DomainError, ex.Message);
            var transactionalResult = new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, result);
            return transactionalResult;
        }
    }
}