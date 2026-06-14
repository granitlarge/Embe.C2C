using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Write.ValueObjects;

public record DistanceWriteDto
(
    double Value,
    LengthUnit Unit
);