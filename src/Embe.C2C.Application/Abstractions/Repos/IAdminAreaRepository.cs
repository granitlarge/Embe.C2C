using Embe.C2C.Application.Abstractions.Entities;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IAdminAreaRepository : IGenericRepository<IAdminArea, string>
{
    public Task<List<IAdminArea>> SearchAdminAreasAsync
    (
        string? parentId,
        double? longitude,
        double? latitude,
        int page,
        int size,
        CancellationToken cancellationToken = default
    );
    public Task<List<IAdminArea>> ReverseGeocodeAsync(double longitude, double latitude, CancellationToken cancellationToken);
    public Task<List<IAdminArea>> GetCountriesAsync(CancellationToken cancellationToken);
}