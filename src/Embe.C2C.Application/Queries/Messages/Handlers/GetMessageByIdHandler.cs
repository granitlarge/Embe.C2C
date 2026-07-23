using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;

namespace Embe.C2C.Application.Queries.Messages.Handlers;

public class GetMessageByIdHandler
(
    IMessageRepository messageRepo,
    MessageAuthorizationService messageAuthorizationService,
    MessageDtoMapper messageDtoMapper
)
{
    private readonly MessageAuthorizationService _messageAuthorizationService = messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper = messageDtoMapper;
    private readonly IMessageRepository _messageRepo = messageRepo;

    public async Task<Result<ReadDto<MessageDto, MessagePermission>>> HandleAsync
    (
        GetMessageByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var permissions = await _messageAuthorizationService.GetPermissionsAsync(query.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.View))
        {
            return Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "You don't have permission to view this message.");
        }

        var message = await _messageRepo.GetMessageByIdIncludeReplyAsync(query.MessageId, cancellationToken);
        var dto = await message.ToDtoAsync(_messageAuthorizationService, _messageDtoMapper, cancellationToken);
        if (dto is null)
        {
            return Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Message not found.");
        }

        return Result<ReadDto<MessageDto, MessagePermission>>.Success(dto);
    }
}