using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record Distance
{

    public static ErrorOr<Distance> Create(double value, LengthUnit unit)
    {
        if (value < 0)
        {
            return DistanceErrors.NegativeDistance.ToValidationErrorOr();
        }
        return new Distance(value, unit);
    }

    private Distance(double value, LengthUnit unit)
    {
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