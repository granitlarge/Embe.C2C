using System.ComponentModel.DataAnnotations.Schema;
using Embe.C2C.Application.Abstractions.Entities;
using NetTopologySuite.Geometries;

namespace Embe.C2C.Infrastructure.Ef.Entities;

public class AdminArea : IAdminArea
{
    public string Id { get; private set; } = null!;

    public string? ParentId { get; private set; }

    public int Level { get; private set; }

    public Point Point { get; private set; } = null!;

    [NotMapped]
    public double Longitude
    {
        get => Point.X;
    }

    [NotMapped]
    public double Latitude
    {
        get => Point.Y;
    }

    public string Name { get; private set; } = null!;

    public string Type { get; private set; } = null!;

    public string EngType { get; private set; } = null!;
}