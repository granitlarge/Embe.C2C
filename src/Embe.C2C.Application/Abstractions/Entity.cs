namespace Embe.C2C.Application.Abstractions;

public record EntityWithPermissions<T_Entity, T_Permission>(T_Entity Entity, T_Permission Permission);