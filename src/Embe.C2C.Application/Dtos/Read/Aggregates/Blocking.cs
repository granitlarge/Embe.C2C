using Embe.C2C.Domain.Aggregates.Blockings;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record BlockingDto
(
    Guid Id,
    Guid BlockerUserId,
    Guid BlockedUserId,
    DateTimeOffset BlockedAt
);

public static class BlockingDtoExtensions
{
    public static BlockingDto ToDto(this Blocking blocking)
    {
        return new BlockingDto
        (
            blocking.Id,
            blocking.BlockerUserId,
            blocking.BlockedUserId,
            blocking.BlockedAt
        );
    }
}
