using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects.Engagements.Enums;

namespace Embe.C2C.Domain.ValueObjects.Engagements;

public record Engagement
{
    public Engagement
    (
        EngagementMedium medium,
        EngagementBoundedness boundedness,
        EngagementFrequency frequency,
        DateOnly? startDate,
        DateOnly? endDate
    )
    {

        if (
            boundedness == EngagementBoundedness.OneTime && frequency != EngagementFrequency.Once ||
            boundedness != EngagementBoundedness.OneTime && frequency == EngagementFrequency.Once
        )
        {
            throw new DomainException(new DomainError<EngagementDomainError>(EngagementDomainError.OneTimeEngagementMustHaveOnceFrequency));
        }

        if (boundedness == EngagementBoundedness.FixedTerm)
        {
            if (startDate is null || endDate is null)
            {
                throw new DomainException(new DomainError<EngagementDomainError>(EngagementDomainError.FixedTermRequiresStartAndEndDate));
            }

            if (startDate > endDate)
            {
                throw new DomainException(new DomainError<EngagementDomainError>(EngagementDomainError.FixedTermStartDateAfterEndDate));
            }
        }
        else
        {
            if (startDate is not null || endDate is not null)
            {
                throw new DomainException(new DomainError<EngagementDomainError>(EngagementDomainError.StartDateAndEndDateMustBeNullForNonFixedTerm));
            }
        }

        Medium = medium;
        Boundedness = boundedness;
        Frequency = frequency;
        StartDate = startDate;
        EndDate = endDate;
    }

    private Engagement()
    {
    }

    public EngagementMedium Medium { get; private set; }
    public EngagementBoundedness Boundedness { get; private set; }
    public EngagementFrequency Frequency { get; private set; }
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
}