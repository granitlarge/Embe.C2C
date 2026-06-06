using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.ValueObjects;

public record DistanceDto
(
    double Value,
    LengthUnit Unit
);

public static class DistanceDtoExtensions
{
    public static DistanceDto ToDto(this Distance distance)
    {
        return new DistanceDto(distance.Value, distance.Unit);
    }
}