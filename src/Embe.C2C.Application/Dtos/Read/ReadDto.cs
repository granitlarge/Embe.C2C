using System.Collections.Immutable;

namespace Embe.C2C.Application.Dtos.Read;

public record ReadDto<T_Data, T_Permissions>
(
    T_Data Data,
    ImmutableHashSet<T_Permissions> Permissions
);