using Embe.C2C.Application.Abstractions;
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

    public async Task<Result<List<IAdminArea>>> HandleAsync(CancellationToken cancellationToken)
    {
        return Result<List<IAdminArea>>.Success(await _repository.GetCountriesAsync(cancellationToken));
    }
}