using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class SearchAdminAreaHandler
{
    private readonly IRepository _repository;

    public SearchAdminAreaHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<IAdminArea>>> HandleAsync(SearchAdminAreaQuery query)
    {
        var adminAreas = await _repository.SearchAdminAreasAsync
        (
            query.ParentId,
            query.Longitude,
            query.Latitude,
            query.Page,
            query.Size
        );
        return Result<List<IAdminArea>>.Success(adminAreas);
    }
}