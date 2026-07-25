using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using ErrorOr;

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

    public async Task<ErrorOr<ReadDto<MessageDto, MessagePermission>>> HandleAsync
    (
        GetMessageByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var permissions = await _messageAuthorizationService.GetPermissionsAsync(query.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.View))
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        var message = await _messageRepo.GetMessageByIdIncludeReplyAsync(query.MessageId, cancellationToken);
        if (message is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var dto = await message.ToDtoAsync(_messageAuthorizationService, _messageDtoMapper, cancellationToken);
        if (dto is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        return dto;
    }
}