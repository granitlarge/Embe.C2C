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
            throw new DomainException("Latitude must be between -90 and 90.");
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new DomainException("Longitude must be between -180 and 180.");
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; }
    public double Longitude { get; }
}