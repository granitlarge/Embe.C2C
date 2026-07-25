namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class LocationErrors
{
    public static readonly DomainError InvalidLatitude = new("location.invalid_latitude", "Latitude must be between -90 and 90.");
    public static readonly DomainError InvalidLongitude = new("location.invalid_longitude", "Longitude must be between -180 and 180.");
}
