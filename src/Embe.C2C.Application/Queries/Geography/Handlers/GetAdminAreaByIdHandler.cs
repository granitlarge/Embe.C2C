using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class GetAdminAreaByIdHandler
{
    private readonly IRepository _repository;

    public GetAdminAreaByIdHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IAdminArea>> HandleAsync(GetAdminAreaByIdQuery query)
    {
        var adminArea = await _repository.AdminAreasQuery
            .AsNoTracking()
            .FirstOrDefaultAsync(aa => aa.Id == query.Id);

        if (adminArea == null)
        {
            return Result<IAdminArea>.Failure(FailureReason.NotFound, $"Admin area with id {query.Id} not found.");
        }

        return Result<IAdminArea>.Success(adminArea);
    }
}