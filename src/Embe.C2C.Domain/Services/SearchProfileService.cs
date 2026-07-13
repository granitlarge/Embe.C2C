using System.Collections.Immutable;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;

namespace Embe.C2C.Domain.Services;

public class SearchProfileService : DomainService
{
    private readonly DomainEventStore _domainEventStore;

    public SearchProfileService(DomainEventStore domainEventStore)
    {
        _domainEventStore = domainEventStore;
    }

    public SearchProfile Create
    (
        User owner,
        string name,
        string description,
        RelationshipType relationshipType,
        Engagement engagement,
        ImmutableHashSet<Gender> genders,
        Age? ageRangeMin,
        Age? ageRangeMax,
        Distance? maximumDistance
    )
    {
        if (maximumDistance != null && owner.Location == null)
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.OwnerLocationNotSet));
        }

        return SearchProfile.Create
        (
            owner.Id,
            name,
            description,
            relationshipType,
            engagement,
            genders,
            ageRangeMin,
            ageRangeMax,
            maximumDistance
        );
    }

    public void Update
    (
        User owner,
        SearchProfile searchProfile,
        string newName,
        string newDescription,
        RelationshipType newRelationshipType,
        Engagement newEngagement,
        ImmutableHashSet<Gender> newGenders,
        Age? newAgeRangeMin,
        Age? newAgeRangeMax,
        Distance? newMaximumDistance,
        bool newActive
    )
    {
        if (newMaximumDistance != null && owner.Location == null)
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.OwnerLocationNotSet));
        }

        searchProfile.ChangeName(newName);
        searchProfile.ChangeDescription(newDescription);
        searchProfile.ChangeRelationshipType(newRelationshipType);
        searchProfile.ChangeEngagement(newEngagement);
        searchProfile.ChangeGenders(newGenders);
        searchProfile.ChangeAgeRange(newAgeRangeMin, newAgeRangeMax);
        searchProfile.ChangeMaximumDistance(newMaximumDistance);

        if (newActive != searchProfile.Active)
        {
            searchProfile.ToggleActive();
        }

    }
}