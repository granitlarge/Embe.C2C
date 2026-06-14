using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Matchings.Handlers;

public class GetMatchingByIdHandler : TransactionalQueryHandler<GetMatchingByIdQuery, Result<ReadDto<MatchingDto, MatchingPermission>>>
{
    private readonly IFileService _fileService;
    private readonly MatchingAuthorizationPolicy _authorizationPolicy;

    public GetMatchingByIdHandler
    (
        IRepository repository,
        IFileService fileService,
        MatchingAuthorizationPolicy authorizationPolicy
    ) : base(repository)
    {
        _fileService = fileService;
        _authorizationPolicy = authorizationPolicy;
    }

    protected override async Task<Result<ReadDto<MatchingDto, MatchingPermission>>> ExecuteAsync(GetMatchingByIdQuery query, ISparseRepository repository, CancellationToken cancellationToken = default)
    {
        var fileGenerator = new FileUrlGenerator(_fileService, TimeSpan.FromMinutes(15));
        var permissions = await _authorizationPolicy.GetPermissionsAsync(query.MatchingId, cancellationToken);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return Result<ReadDto<MatchingDto, MatchingPermission>>.Failure(FailureReason.Forbidden, "You do not have permission to view this matching.");
        }

        var matching = await repository
            .MatchingsQuery
            .AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.Messages!.OrderByDescending(m => m.CreatedAt).Take(50))
            .SingleOrDefaultAsync(m => m.Id == query.MatchingId, cancellationToken);

        if (matching == null)
        {
            return Result<ReadDto<MatchingDto, MatchingPermission>>.Failure(FailureReason.NotFound, "Matching not found.");
        }

        var dto = await _authorizationPolicy.ToDtoAsync(matching, cancellationToken);
        if (dto == null)
        {
            return Result<ReadDto<MatchingDto, MatchingPermission>>.Failure(FailureReason.Forbidden, "You do not have permission to view this matching.");
        }

        return Result<ReadDto<MatchingDto, MatchingPermission>>.Success(dto);
    }
}