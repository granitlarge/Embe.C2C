using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class GetCountryAdminAreaHandler
{
    private readonly IRepository _repository;

    public GetCountryAdminAreaHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<IAdminArea>>> HandleAsync()
    {
        return Result<List<IAdminArea>>.Success(await _repository.AdminAreasQuery
            .AsNoTracking()
            .Where(aa => aa.ParentId == null)
            .ToListAsync());
    }
}