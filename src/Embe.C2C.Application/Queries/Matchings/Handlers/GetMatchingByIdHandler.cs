using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

public class GetMatchingByIdHandler : TransactionalQueryHandler<GetMatchingByIdQuery, Result<MatchingDto>>
{
    private readonly IAuthenticatedUserService _user;
    private readonly IFileService _fileService;

    public GetMatchingByIdHandler(IAuthenticatedUserService userService, IRepository repository, IFileService fileService) : base(repository)
    {
        _user = userService;
        _fileService = fileService;
    }

    protected override async Task<Result<MatchingDto>> ExecuteAsync(GetMatchingByIdQuery query, ISparseRepository repository, CancellationToken cancellationToken = default)
    {
        var userId = _user.UserId ?? throw new UnauthorizedAccessException("User must be authenticated to get a matching.");
        var matching = await repository
            .MatchingsQuery
            .AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.Messages!.OrderByDescending(m => m.CreatedAt).Take(50))
            .SingleOrDefaultAsync(m => m.Id == query.MatchingId && (m.UserId1 == userId || m.UserId2 == userId), cancellationToken);

        if (matching == null)
        {
            return Result<MatchingDto>.Failure(FailureReason.NotFound, "Matching not found.");
        }

        var fileGenerator = new FileUrlGenerator(_fileService, TimeSpan.FromMinutes(15));
        return Result<MatchingDto>.Success(await matching.ToDtoAsync(userId, fileGenerator, cancellationToken));
    }
}