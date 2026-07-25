using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class EditMessageHandler : CommandHandler<EditMessageCommand, ErrorOr<ReadDto<MessageDto, MessagePermission>>>
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

    protected async override Task<CommandResult<ErrorOr<ReadDto<MessageDto, MessagePermission>>>> InternalHandleAsync
    (
        EditMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _messageAuthorizationService.GetPermissionsAsync(command.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.Edit))
        {
            return new(false, ApplicationErrors.Forbidden.ToForbiddenErrorOr());
        }

        var user = await _userRepo.GetByIdAsync(_authenticatedUser.UserId ?? throw new InvalidOperationException("user is not authenticated"), cancellationToken);
        if (user is null)
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());

        var message = await _messageRepo.GetMessageByIdIncludeReplyAsync(command.MessageId, cancellationToken);
        if (message is null)
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());

        var messageContent = MessageContent.Create(command.NewContent);
        if (messageContent.IsError)
        {
            return new
            (
                false,
                ErrorOrFactory.From<ReadDto<MessageDto, MessagePermission>>(messageContent.Errors.WithPropertyName(nameof(command.NewContent)))
            );
        }

        _matchingService.EditMessage(user, message, messageContent.Value);

        var readDto = await message.ToDtoAsync(_messageAuthorizationService, _messageDtoMapper, cancellationToken);
        if (readDto == null)
        {
            return new(false, ApplicationErrors.Forbidden.ToForbiddenErrorOr());
        }

        return new(true, ErrorOrFactory.From(readDto));
    }
}