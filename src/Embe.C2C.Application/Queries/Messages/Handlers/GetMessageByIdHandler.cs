using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Messages.Handlers;

public class GetMessageByIdHandler
(
    IRepository repository,
    MessageAuthorizationPolicy authorizationPolicy
)
{
    private readonly IRepository _repository = repository;
    private readonly MessageAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<ReadDto<MessageDto, MessagePermission>>> HandleAsync
    (
        GetMessageByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(query.MessageId, cancellationToken);
        if (!permissions.Contains(MessagePermission.View))
        {
            return Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.Forbidden, "You don't have permission to view this message.");
        }

        var message = await _repository.MessagesQuery
            .Include(m => m.ReplyToMessage)
            .SingleAsync(m => m.Id == query.MessageId, cancellationToken);

        var dto = await _authorizationPolicy.ToDtoAsync(message, cancellationToken);
        if (dto is null)
        {
            return Result<ReadDto<MessageDto, MessagePermission>>.Failure(FailureReason.NotFound, "Message not found.");
        }

        return Result<ReadDto<MessageDto, MessagePermission>>.Success(dto);
    }
}