using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class SearchProfileExtensions
{
    public static async Task<ReadDto<SearchProfileDto, SearchProfilePermission>?> ToDtoAsync
    (
        this SearchProfile searchProfile,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await searchProfileAuthorizationService.GetAsync(searchProfile, cancellationToken);
        var dto = searchProfileDtoMapper.ToDto(searchProfile, variant);
        if (dto is null)
            return null;
        return new ReadDto<SearchProfileDto, SearchProfilePermission>(dto, permissions);
    }
}