using Embe.C2C.Domain.Aggregates.Contacts;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record ContactDto
(
    Guid Id,
    Guid UserId1,
    Guid UserId2,
    DateTimeOffset CreatedAt
);

public static class ContactDtoExtensions
{
    public static ContactDto ToDto(this Contact contact)
    {
        return new ContactDto
        (
            contact.Id,
            contact.UserId1,
            contact.UserId2,
            contact.CreatedAt
        );
    }
}
