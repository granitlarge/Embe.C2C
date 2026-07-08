using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

/**
    1. Figure out which matchings the user is allowed to see.
    2. Figure out which related entities the user is allowed to see.
    3. Figure out which permissions the user has for the matching.
    4. Figure out the slice of information to return based on the permissions.
*/

public class GetMatchingsHandler
(
    IRepository repository,
    IFileService fileService,
    MatchingAuthorizationService matchingAuthorizationService,
    MatchingDtoMapper matchingDtoMapper,
    UserAuthorizationService userAuthorizationService,
    UserDtoMapper userDtoMapper,
    MessageAuthorizationService messageAuthorizationService,
    MessageDtoMapper messageDtoMapper,
    ConversationDtoMapper conversationDtoMapper
) : TransactionalQueryHandler<GetMatchingsQuery, Result<List<ReadDto<MatchingDto, MatchingPermission>>>>(repository)
{
    private readonly IFileService _fileService = fileService;
    private readonly MatchingAuthorizationService _matchingAuthorizationService = matchingAuthorizationService;
    private readonly MatchingDtoMapper _matchingDtoMapper = matchingDtoMapper;
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly MessageAuthorizationService _messageAuthorizationService = messageAuthorizationService;
    private readonly MessageDtoMapper _messageDtoMapper = messageDtoMapper;
    private readonly ConversationDtoMapper _conversationDtoMapper = conversationDtoMapper;

    protected override async Task<Result<List<ReadDto<MatchingDto, MatchingPermission>>>> ExecuteAsync
    (
        GetMatchingsQuery query,
        ISparseRepository _,
        CancellationToken cancellationToken
    )
    {
        var viewable = _matchingAuthorizationService.GetViewable();
        var matchings = await viewable
            .AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.LastMessage)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync(cancellationToken);

        var dtos = new List<ReadDto<MatchingDto, MatchingPermission>>();
        foreach (var matching in matchings)
        {
            var readDto = await matching.ToDtoAsync
            (
                matching.User1,
                matching.User2,
                _matchingAuthorizationService,
                _matchingDtoMapper,
                _userAuthorizationService,
                _userDtoMapper,
                _messageAuthorizationService,
                _messageDtoMapper,
                _conversationDtoMapper,
                cancellationToken
            );

            if (readDto is not null)
            {
                dtos.Add(readDto);
            }
        }

        return Result<List<ReadDto<MatchingDto, MatchingPermission>>>.Success(dtos);
    }
}