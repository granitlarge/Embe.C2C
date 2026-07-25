namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class DistanceErrors
{
    public static readonly DomainError NegativeDistance = new("distance.negative", "Distance cannot be negative");
}
