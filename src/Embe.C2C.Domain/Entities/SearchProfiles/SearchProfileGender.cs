using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Entities.SearchProfiles;

public class SearchProfileGender : Entity
{
    private SearchProfileGender
    (
        Guid searchProfileId,
        Gender gender
    )
    {
        Id = Guid.CreateVersion7();
        SearchProfileId = searchProfileId;
        Gender = gender;
    }

    private SearchProfileGender()
    {

    }

    public Guid Id { get; }
    public Guid SearchProfileId { get; }
    public Gender Gender { get; }

    internal static SearchProfileGender Create(Guid searchProfileId, Gender gender)
    {
        return new SearchProfileGender(searchProfileId, gender);
    }
}