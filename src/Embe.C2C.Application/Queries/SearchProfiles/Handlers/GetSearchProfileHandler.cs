using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using ErrorOr;

namespace Embe.C2C.Application.Queries.SearchProfiles.Handlers;

public class GetSearchProfileHandler
(
    ISearchProfileRepository searchProfileRepository,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper
)
{
    private readonly ISearchProfileRepository _searchProfileRepository = searchProfileRepository;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;

    public async Task<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>> HandleAsync
    (
        GetSearchProfileQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, _) = await _searchProfileAuthorizationService.GetAsync(query.Id, cancellationToken);
        if (!permissions.Contains(SearchProfilePermission.View))
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        var searchProfile = await _searchProfileRepository.GetByIdAsync(query.Id, cancellationToken);
        if (searchProfile is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var dto = await _searchProfileDtoMapper.ToDtoAsync(searchProfile, cancellationToken);
        if (dto is null)
        {
            return ApplicationErrors.Forbidden.ToForbiddenErrorOr();
        }

        return dto;
    }
}