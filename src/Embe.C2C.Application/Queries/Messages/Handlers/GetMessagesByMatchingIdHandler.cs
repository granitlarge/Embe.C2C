using System.Text.Json;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Messages.Handlers;

public class GetMessagesByMatchingIdHandler
{
    private readonly IRepository _repository;
    private readonly MessageAuthorizationService _messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper;

    public GetMessagesByMatchingIdHandler
    (
        IRepository repository,
        MessageAuthorizationService messageAuthorizationPolicy,
        MessageDtoMapper messageDtoMapper
    )
    {
        _repository = repository;
        _messageAuthorizationService = messageAuthorizationPolicy;
        _messageDtoMapper = messageDtoMapper;
    }

    public async Task<Result<List<ReadDto<MessageDto, MessagePermission>>>> HandleAsync(GetMessagesByMatchingIdQuery query, CancellationToken cancellationToken)
    {
        var messages = await _repository.MessagesQuery
            .Where(m => m.Conversation!.Matching!.Id == query.Filter)
                .Include(m => m.ReplyToMessage)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync(cancellationToken);

        var dtos = new List<ReadDto<MessageDto, MessagePermission>>();
        foreach (var message in messages)
        {
            var (permissions, variant) = await _messageAuthorizationService.GetAsync(message, cancellationToken);
            var dto = _messageDtoMapper.ToDto(message, variant);
            if (dto != null)
                dtos.Add(new ReadDto<MessageDto, MessagePermission>(dto, permissions));
        }

        return Result<List<ReadDto<MessageDto, MessagePermission>>>.Success(dtos);
    }
}