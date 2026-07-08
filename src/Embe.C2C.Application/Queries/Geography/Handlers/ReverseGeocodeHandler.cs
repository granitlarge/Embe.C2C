using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class ReverseGeocodeHandler
{
    private readonly IRepository _repository;

    public ReverseGeocodeHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<IAdminArea>>> HandleAsync(ReverseGeocodeQuery query)
    {
        var result = await _repository.ReverseGeocodeAsync(query.Longitude, query.Latitude);
        return Result<List<IAdminArea>>.Success(result);
    }
}