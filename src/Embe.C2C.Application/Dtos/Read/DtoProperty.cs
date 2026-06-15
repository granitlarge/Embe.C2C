namespace Embe.C2C.Application.Dtos.Read;

public record DtoProperty<T>
(
    bool CanView,
    T? Value
) where T : notnull;