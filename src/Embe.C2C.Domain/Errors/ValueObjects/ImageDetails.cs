namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class ImageDetailsErrors
{
    public static readonly DomainError NegativeOrder = new("image.order_negative", "Order must be greater than or equal to 0.");
}
