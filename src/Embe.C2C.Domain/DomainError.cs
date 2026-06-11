namespace Embe.C2C.Domain;

public record DomainError(string Group, object Value);
public record DomainError<T>(T ErrorCode) : DomainError(typeof(T).Name, ErrorCode) where T : Enum;