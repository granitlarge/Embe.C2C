using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Messages.Handlers;

public class GetMessagesByMatchingIdHandler
{
    private readonly IMessageRepository _messageRepo;
    private readonly MessageAuthorizationService _messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper;

    public GetMessagesByMatchingIdHandler
    (
        IMessageRepository messageRepo,
        MessageAuthorizationService messageAuthorizationPolicy,
        MessageDtoMapper messageDtoMapper
    )
    {
        _messageAuthorizationService = messageAuthorizationPolicy;
        _messageDtoMapper = messageDtoMapper;
        _messageRepo = messageRepo;
    }

    public async Task<ErrorOr<List<ReadDto<MessageDto, MessagePermission>>>> HandleAsync(GetMessagesByMatchingIdQuery query, CancellationToken cancellationToken)
    {
        var messages = await _messageRepo.GetMessagesByMatchingIdAsync(query.Filter, query.Page, query.Size, cancellationToken);
        var dtos = new List<ReadDto<MessageDto, MessagePermission>>();
        foreach (var message in messages)
        {
            var readDto = await _messageDtoMapper.ToDtoAsync(message, cancellationToken);
            if (readDto != null)
            {
                dtos.Add(readDto);
            }
        }

        return dtos;
    }
}