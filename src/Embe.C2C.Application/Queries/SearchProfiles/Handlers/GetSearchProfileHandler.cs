using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.SearchProfiles.Handlers;

public class GetSearchProfileHandler
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IRepository _context;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper;

    public GetSearchProfileHandler
    (
        IAuthenticatedUserService authenticatedUserService,
        IRepository context,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper
    )
    {
        _authenticatedUserService = authenticatedUserService;
        _context = context;
        _searchProfileAuthorizationService = searchProfileAuthorizationService;
        _searchProfileDtoMapper = searchProfileDtoMapper;
    }

    public async Task<Result<ReadDto<SearchProfileDto, SearchProfilePermission>>> HandleAsync
    (
        GetSearchProfileQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await _searchProfileAuthorizationService.GetAsync(query.Id, cancellationToken);
        if (!permissions.Contains(SearchProfilePermission.View))
        {
            return Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Failure
            (
                FailureReason.Forbidden,
                "You do not have permission to view this search profile."
            );
        }

        var searchProfile = await _context.SearchProfilesQuery.FirstOrDefaultAsync(sp => sp.Id == query.Id, cancellationToken: cancellationToken);
        if (searchProfile is null)
        {
            return Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Failure
            (
                FailureReason.NotFound,
                "Search profile not found."
            );
        }

        var dto = await searchProfile.ToDtoAsync(_searchProfileAuthorizationService, _searchProfileDtoMapper, cancellationToken);
        if (dto is null)
        {
            return Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Failure
            (
                FailureReason.Forbidden,
                "User does not have permission to view this search profile."
            );
        }

        return Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Success(dto);
    }
}