using Embe.C2C.Domain.Aggregates.ContactRequests;

namespace Embe.C2C.Application.Dtos.Aggregates;

public record ContactRequestDto
(
    Guid Id,
    Guid RequestorUserId,
    Guid RecipientUserId,
    bool? IsAccepted,
    DateTimeOffset? RespondedAt,
    DateTimeOffset RequestedAt
);

public static class ContactRequestDtoExtensions
{
    public static ContactRequestDto ToDto(this ContactRequest contactRequest)
    {
        return new ContactRequestDto
        (
            contactRequest.Id,
            contactRequest.RequestorUserId,
            contactRequest.RecipientUserId,
            contactRequest.IsAccepted,
            contactRequest.RespondedAt,
            contactRequest.RequestedAt
        );
    }
}
