using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Geography.Handlers;

public class GetAdminAreaByIdHandler
{
    private readonly IAdminAreaRepository _repository;

    public GetAdminAreaByIdHandler(IAdminAreaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<IAdminArea>> HandleAsync(GetAdminAreaByIdQuery query, CancellationToken cancellationToken)
    {
        var adminArea = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (adminArea == null)
        {
            return Error.NotFound("not_found", $"Admin area with id {query.Id} not found.");
        }

        return ErrorOrFactory.From(adminArea);
    }
}