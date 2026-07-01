using Embe.C2C.Domain.ValueObjects.Engagements.Enums;

namespace Embe.C2C.Application.Dtos.Write.ValueObjects;

public record EngagementWriteDto
(
    EngagementMedium Medium,
    EngagementBoundedness Boundedness,
    EngagementFrequency Frequency,
    DateOnly? StartDate,
    DateOnly? EndDate
);