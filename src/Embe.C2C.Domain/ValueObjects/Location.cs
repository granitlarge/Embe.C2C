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
}

public enum LocationError
{
    InvalidLatitude,
    InvalidLongitude
}