using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Domain.Aggregates.Matchings;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

public class GetMatchingsHandler(IC2CContext context, IAuthenticatedUserService userService)
{
    private readonly IC2CContext _context = context;
    private readonly IAuthenticatedUserService _userService = userService;

    public async Task<Result<List<Matching>>> HandleAsync
    (
        GetMatchingsQuery query,
        CancellationToken cancellationToken
    )
    {
        var userId = _userService.UserId ?? throw new InvalidOperationException();
        var matchings = await _context.Matchings
            .Where(m => m.UserId1 == userId || m.UserId2 == userId)
            .ToListAsync(cancellationToken);

        return Result<List<Matching>>.Success(matchings);
    }
}