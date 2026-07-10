using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.SearchProfiles.Handlers;

public class GetAllSearchProfilesHandler
{
    private readonly IRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper;

    public GetAllSearchProfilesHandler
    (
        IRepository repository,
        IAuthenticatedUserService authenticatedUserService,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper
    )
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
        _searchProfileAuthorizationService = searchProfileAuthorizationService;
        _searchProfileDtoMapper = searchProfileDtoMapper;
    }

    public async Task<Result<List<ReadDto<SearchProfileDto, SearchProfilePermission>>>> HandleAsync
    (
        GetAllSearchProfilesQuery query,
        CancellationToken cancellationToken
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var searchProfiles = await _repository.SearchProfilesQuery
        .AsNoTracking()
        .Where(sp => sp.UserId == userId)
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync(cancellationToken);

        var dtos = new List<ReadDto<SearchProfileDto, SearchProfilePermission>>();
        foreach (var searchProfile in searchProfiles)
        {
            var dto = await searchProfile.ToDtoAsync(_searchProfileAuthorizationService, _searchProfileDtoMapper, cancellationToken);
            if (dto != null)
                dtos.Add(dto);
        }

        return Result<List<ReadDto<SearchProfileDto, SearchProfilePermission>>>.Success(dtos);
    }
}