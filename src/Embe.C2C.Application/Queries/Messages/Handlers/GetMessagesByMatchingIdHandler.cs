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
    private readonly MatchingAuthorizationPolicy _matchingAuthorizationPolicy;
    private readonly MessageAuthorizationPolicy _messageAuthorizationPolicy;

    public GetMessagesByMatchingIdHandler
    (
        IRepository repository,
        MatchingAuthorizationPolicy matchingAuthorizationPolicy,
        MessageAuthorizationPolicy messageAuthorizationPolicy
    )
    {
        _repository = repository;
        _matchingAuthorizationPolicy = matchingAuthorizationPolicy;
        _messageAuthorizationPolicy = messageAuthorizationPolicy;
    }

    public async Task<Result<List<ReadDto<MessageDto, MessagePermission>>>> HandleAsync(GetMessagesByMatchingIdQuery query, CancellationToken cancellationToken)
    {
        var messages = await _repository.MessagesQuery
            .Where(m => m.Conversation!.Matching!.Id == query.Filter)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync(cancellationToken);

        var dtos = new List<ReadDto<MessageDto, MessagePermission>>();
        foreach (var message in messages)
        {
            var dto = await _messageAuthorizationPolicy.ToDtoAsync(message, cancellationToken);
            if (dto != null)
                dtos.Add(dto);
        }

        return Result<List<ReadDto<MessageDto, MessagePermission>>>.Success(dtos);
    }
}