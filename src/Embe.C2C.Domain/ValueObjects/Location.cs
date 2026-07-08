using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Location
{
    public Location
    (
        double latitude,
        double longitude
    )
    {
        if (latitude < -90 || latitude > 90)
        {
            throw new DomainException(new DomainError<LocationError>(LocationError.InvalidLatitude));
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new DomainException(new DomainError<LocationError>(LocationError.InvalidLongitude));
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; }
    public double Longitude { get; }

    public Distance DistanceTo(Location location)
    {
        return new Distance(Haversine(Latitude, Longitude, location.Latitude, location.Longitude) / 1000, LengthUnit.Kilometers);
    }

    private const double EarthRadiusMeters = 6371000.0;

    private static double Haversine(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        lat1 = DegreesToRadians(lat1);
        lat2 = DegreesToRadians(lat2);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1) * Math.Cos(lat2) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    private static double DegreesToRadians(double degrees)
        => degrees * Math.PI / 180.0;
}

public enum LocationError
{
    InvalidLatitude,
    InvalidLongitude
}