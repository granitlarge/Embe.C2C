namespace Embe.C2C.Domain.Exceptions;

public class DomainException(DomainError Error) : Exception($"A domain error occurred. Group: {Error.Group}, Value: {Error.Value}")
{
    public DomainError Error { get; } = Error;
}