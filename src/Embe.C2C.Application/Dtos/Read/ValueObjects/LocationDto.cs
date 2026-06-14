namespace Embe.C2C.Application.Dtos.Read.ValueObjects;

public record LocationDto
(
    double Latitude,
    double Longitude
);

public static class LocationDtoExtensions
{
    public static LocationDto ToDto(this Domain.ValueObjects.Location location)
    {
        return new LocationDto(location.Latitude, location.Longitude);
    }
}