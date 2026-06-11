using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

public class GetMatchingsHandler(IRepository context, IAuthenticatedUserService userService, IFileService fileService)
{
    private readonly IRepository _context = context;
    private readonly IAuthenticatedUserService _userService = userService;
    private readonly IFileService _fileService = fileService;

    public async Task<Result<MatchingDto[]>> HandleAsync
    (
        GetMatchingsQuery query,
        CancellationToken cancellationToken
    )
    {
        var userId = _userService.UserId ?? throw new InvalidOperationException();
        var matchings1 = await _context.MatchingsQuery
            .AsNoTracking()
            .Include(m => m.User2)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.LastMessage)
            .Where(m => m.UserId1 == userId)
            .ToListAsync(cancellationToken);

        var matchings2 = await _context.MatchingsQuery
            .AsNoTracking()
            .Include(m => m.User1)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.LastMessage)
            .Where(m => m.UserId2 == userId)
            .ToListAsync(cancellationToken);

        var matchings = matchings1.Concat(matchings2).ToList();

        var urlGenerator = new FileUrlGenerator(_fileService, TimeSpan.FromSeconds(15));
        var dtos = await Task.WhenAll(matchings.Select(matching => matching.ToDto(urlGenerator)));

        return Result<MatchingDto[]>.Success(dtos);
    }
}