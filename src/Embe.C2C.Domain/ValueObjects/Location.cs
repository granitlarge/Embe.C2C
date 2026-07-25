using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record Location
{
    private Location
    (
        double latitude,
        double longitude
    )
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static ErrorOr<Location> Create(double latitude, double longitude)
    {
        var errors = new List<Error>();
        if (latitude < -90 || latitude > 90)
        {
            errors.Add(LocationErrors.InvalidLatitude.ToValidationErrorOr());
        }

        if (longitude < -180 || longitude > 180)
        {
            errors.Add(LocationErrors.InvalidLongitude.ToValidationErrorOr());
        }

        if (errors.Count != 0)
            return errors;

        return new Location(latitude, longitude);
    }

    public double Latitude { get; }
    public double Longitude { get; }

    public Distance DistanceTo(Location location)
    {
        return Distance.Create(Haversine(Latitude, Longitude, location.Latitude, location.Longitude) / 1000, LengthUnit.Kilometers).Value;
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