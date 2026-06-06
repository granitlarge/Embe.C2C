using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Distance
{
    public Distance(double value, LengthUnit unit)
    {
        if (value < 0)
        {
            throw new DomainException("Distance cannot be negative.");
        }

        Value = value;
        Unit = unit;
    }

    public double Value { get; }
    public LengthUnit Unit { get; }

    public Distance ToKilometers()
    {
        return Unit switch
        {
            LengthUnit.Kilometers => this,
            LengthUnit.Miles => new Distance(Value * 1.60934, LengthUnit.Kilometers),
            _ => throw new InvalidOperationException("Unknown length unit.")
        };
    }

    public Distance ToMiles()
    {
        return Unit switch
        {
            LengthUnit.Kilometers => new Distance(Value / 1.60934, LengthUnit.Miles),
            LengthUnit.Miles => this,
            _ => throw new InvalidOperationException("Unknown length unit.")
        };
    }
}

public enum LengthUnit
{
    Kilometers = 0,
    Miles = 1
}