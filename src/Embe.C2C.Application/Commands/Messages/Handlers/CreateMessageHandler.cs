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
using Embe.C2C.Domain.Policies;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class CreateMessageHandler
(
    IBlockingRepository blockingRepo,
    IMessageRepository messageRepo,
    IMatchingRepository matchingRepo,
    IUserRepository userRepo,
    DomainEventStore domainEventStore,
    MatchingAuthorizationService matchingAuthorizationService,
    MessageAuthorizationService messageAuthorizationService,
    IAuthenticatedUserService authenticatedUser,
    MatchingService matchingService,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    MessageDtoMapper messageDtoMapper
) : CommandHandler<CreateMessageCommand, Result<ReadDto<MessageDto, MessagePermission>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IBlockingRepository _blockingRepo = blockingRepo;
    private readonly IMessageRepository _messageRepo = messageRepo;
    private readonly IMatchingRepository _matchingRepo = matchingRepo;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly MatchingAuthorizationService _matchingAuthorizationService = matchingAuthorizationService;
    private readonly MessageAuthorizationService _messageAuthorizationService = messageAuthorizationService;
    private readonly IAuthenticatedUserService _authenticatedUser = authenticatedUser;
    private readonly MatchingService _matchingService = matchingService;
    private readonly MessageDtoMapper _messageDtoMapper = messageDtoMapper;

    protected async override Task<CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>> InternalHandleAsync
    (
        CreateMessageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await _matchingAuthorizationService.GetPermissionsAsync(command.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.Chat))
        {
            return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "You don't have permission to send messages in this matching."));
        }

        try
        {
            var messageContent = MessageContent.Create(command.Content);
            var replyToMessageId = command.ReplyToMessageId;

            var user = await _userRepo.GetByIdAsync(_authenticatedUser.UserId ?? throw new InvalidOperationException("user is not authenticated"), cancellationToken);
            if (user is null)
            {
                return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "Authenticated user not found."));
            }

            var matching = await _matchingRepo.GetByIdAsync(command.MatchingId, cancellationToken);
            if (matching is null)
            {
                return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Matching not found."));
            }

            var receiver = await _userRepo.GetByIdAsync(matching.GetOtherUserId(_authenticatedUser.UserId)!.Value, cancellationToken);
            if (receiver is null)
            {
                return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Receiver user not found."));
            }

            var replyToMessage = replyToMessageId.HasValue ? await _messageRepo.GetByIdAsync(replyToMessageId.Value, cancellationToken) : null;
            if (replyToMessageId.HasValue && replyToMessage is null)
            {
                return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Reply-to message not found."));
            }

            var blocking1 = await _blockingRepo.GetByUserIdsAsync(user.Id, receiver.Id, cancellationToken);
            var blocking2 = await _blockingRepo.GetByUserIdsAsync(receiver.Id, user.Id, cancellationToken);

            var communicationPolicy = new CommunicationPolicy(user, receiver, matching, blocking1, blocking2);
            var message = _matchingService.SendMessage(user, matching, messageContent, communicationPolicy, replyToMessage);

            _messageRepo.Set.Add(message);

            var readDto = await message.ToDtoAsync(_messageAuthorizationService, _messageDtoMapper, cancellationToken) ??
                throw new InvalidOperationException("The message should be viewable to the sender right after sending it.");

            var result = Result<ReadDto<MessageDto, MessagePermission>>.Success(readDto);
            return new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(true, result);
        }
        catch (DomainException ex)
        {
            var result = Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.DomainError, ex.Message);
            var transactionalResult = new CommandResult<Result<ReadDto<MessageDto, MessagePermission>>>(false, result);
            return transactionalResult;
        }
    }
}