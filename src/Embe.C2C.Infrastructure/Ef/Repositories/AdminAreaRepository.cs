using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class AdminAreaRepository(C2CContext context) : IAdminAreaRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<IAdminArea> Set => throw new NotImplementedException();

    public async Task<IAdminArea?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.AdminAreas.SingleOrDefaultAsync(aa => aa.Id == id, cancellationToken);
    }

    public async Task<List<IAdminArea>> GetCountriesAsync(CancellationToken cancellationToken)
    {
        var adminAreas = await _context.AdminAreas.Where(aa => aa.ParentId == null).Cast<IAdminArea>().ToListAsync(cancellationToken);
        return adminAreas;
    }

    public async Task<List<IAdminArea>> ReverseGeocodeAsync(double longitude, double latitude)
    {
        var adminArea = (await SearchAdminAreasAsync(null, longitude, latitude, 1, 1)).FirstOrDefault();
        if (adminArea == null)
        {
            return [];
        }

        var adminAreas = new List<IAdminArea> { adminArea };
        var highestLevelAdminArea = adminAreas[0];
        while (highestLevelAdminArea.ParentId != null)
        {
            var parent = await _context.AdminAreas.AsNoTracking().FirstOrDefaultAsync(aa => aa.Id == highestLevelAdminArea.ParentId);
            if (parent == null)
            {
                break;
            }
            adminAreas.Add(parent);
            highestLevelAdminArea = parent;
        }

        return adminAreas;
    }

    public Task<List<IAdminArea>> ReverseGeocodeAsync(double longitude, double latitude, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<IAdminArea>> SearchAdminAreasAsync
    (
        string? parentId,
        double? longitude,
        double? latitude,
        int page,
        int size,
        CancellationToken cancellationToken = default
    )
    {
        var maxDistanceMeters = 1000;
        List<AdminArea> result = [];
        while (result.Count == 0 && maxDistanceMeters <= 25_000_000)
        {
            var pointFilter = (longitude.HasValue && latitude.HasValue) ? new NetTopologySuite.Geometries.Point(longitude.Value, latitude.Value) { SRID = 4326 } : null;
            var parentIdFilter = !string.IsNullOrEmpty(parentId) ? parentId : null;
            result = await _context.AdminAreas
            .AsNoTracking()
            .Where(aa => pointFilter == null || aa.Point != null && aa.Point.Distance(pointFilter) <= maxDistanceMeters)
            .Where(aa => parentIdFilter == null || aa.ParentId == parentIdFilter)
            .OrderBy(aa => pointFilter != null && aa.Point != null ? aa.Point.Distance(pointFilter) : int.MaxValue)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
            maxDistanceMeters *= 2;
        }

        return [.. result.Cast<IAdminArea>()];
    }
}