using Embe.C2C.Application.Dtos.Aggregates;

namespace Embe.C2C.Application.Dtos.Entities;

public record ConversationDto
(
    Guid Id,
    Guid MatchingId,
    Guid UserId1,
    Guid UserId2,
    Guid? LastMessageId,
    uint MessageCount,
    DateTimeOffset UpdatedAt,
    DateTimeOffset CreatedAt,
    MessageDto? LastMessage
);

public static class ConversationDtoExtensions
{
    public static ConversationDto ToDto(this Domain.Entities.Conversation conversation)
    {
        return new ConversationDto
        (
            conversation.Id,
            conversation.MatchingId,
            conversation.UserId1,
            conversation.UserId2,
            conversation.LastMessageId,
            conversation.MessageCount,
            conversation.UpdatedAt,
            conversation.CreatedAt,
            conversation.LastMessage?.ToDto()
        );
    }
}
