using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
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
    MatchingAuthorizationPolicy authorizationPolicy
) : TransactionalQueryHandler<GetMatchingsQuery, Result<ReadDto<MatchingDto, MatchingPermission>[]>>(repository)
{
    private readonly IFileService _fileService = fileService;
    private readonly MatchingAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    protected override async Task<Result<ReadDto<MatchingDto, MatchingPermission>[]>> ExecuteAsync
    (
        GetMatchingsQuery query,
        ISparseRepository _,
        CancellationToken cancellationToken
    )
    {
        var fileUrlGenerator = new FileUrlGenerator(_fileService, TimeSpan.FromSeconds(15));
        var viewable = _authorizationPolicy.GetViewable();
        var matchings = await viewable
            .AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.LastMessage)
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = await Task.WhenAll(matchings.Select(m => _authorizationPolicy.ToDtoAsync(m, cancellationToken)));
        var notnulls = dtos.Where(dto => dto is not null).Select(dto => dto!).ToArray();
        return Result<ReadDto<MatchingDto, MatchingPermission>[]>.Success(notnulls);
    }
}