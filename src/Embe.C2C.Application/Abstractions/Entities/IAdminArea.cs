namespace Embe.C2C.Application.Abstractions.Entities;

public interface IAdminArea
{
    string Id { get; }
    string? ParentId { get; }
    int Level { get; }
    double Longitude { get; }
    double Latitude { get; }
    string Name { get; }
    string Type { get; }
    string EngType { get; }
}