using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class SearchAdminAreaHandler
{
    private readonly IAdminAreaRepository _repository;

    public SearchAdminAreaHandler(IAdminAreaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IAdminArea>> HandleAsync(SearchAdminAreaQuery query, CancellationToken cancellationToken)
    {
        var adminAreas = await _repository.SearchAdminAreasAsync
        (
            query.ParentId,
            query.Longitude,
            query.Latitude,
            query.Page,
            query.Size,
            cancellationToken
        );
        return adminAreas;
    }
}