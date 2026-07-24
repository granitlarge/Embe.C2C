using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class ReverseGeocodeHandler
{
    private readonly IAdminAreaRepository _adminAreaRepository;

    public ReverseGeocodeHandler(IAdminAreaRepository adminAreaRepository)
    {
        _adminAreaRepository = adminAreaRepository;
    }

    public async Task<Result<List<IAdminArea>>> HandleAsync(ReverseGeocodeQuery query, CancellationToken cancellationToken)
    {
        var result = await _adminAreaRepository.ReverseGeocodeAsync(query.Longitude, query.Latitude, cancellationToken);
        return Result<List<IAdminArea>>.Success(result);
    }
}