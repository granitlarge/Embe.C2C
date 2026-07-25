using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class GetCountryAdminAreaHandler
{
    private readonly IAdminAreaRepository _repository;

    public GetCountryAdminAreaHandler(IAdminAreaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IAdminArea>> HandleAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetCountriesAsync(cancellationToken);
    }
}