using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Errors;
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
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        return ErrorOrFactory.From(adminArea);
    }
}