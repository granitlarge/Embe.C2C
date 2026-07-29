using System.Collections.Immutable;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.SearchProfiles.Events;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;
using ErrorOr;

namespace Embe.C2C.Domain.Services;

public class SearchProfileService(DomainEventStore domainEventStore) : DomainService
{
    private readonly DomainEventStore _domainEventStore = domainEventStore;

    public ErrorOr<SearchProfile> Create
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
            return SearchProfileErrors.OwnerDistanceFilterButLocationNotSet.ToValidationErrorOr(new Dictionary<string, object>
            {
                { "ownerId", owner.Id }
            });
        }

        var searchProfile = SearchProfile.Create
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

        if (searchProfile.IsSuccess)
        {
            _domainEventStore.AddDomainEvent(new SearchProfileCreatedDomainEvent(searchProfile.Value));
        }


        return searchProfile;
    }

    public ErrorOr<SearchProfile> Update
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
            return SearchProfileErrors.OwnerDistanceFilterButLocationNotSet.ToRuleErrorOr(new Dictionary<string, object>
            {
                { "ownerId", owner.Id }
            });
        }

        var errors = new List<Error>();
        var nameChangeResult = searchProfile.ChangeName(newName);
        if (nameChangeResult.IsError)
        {
            errors.AddRange(nameChangeResult.Errors);
        }

        var descriptionIsDifferent = searchProfile.Description != newDescription;
        var descriptionChangeResult = searchProfile.ChangeDescription(newDescription);
        if (descriptionChangeResult.IsError)
        {
            errors.AddRange(descriptionChangeResult.Errors);
        }

        searchProfile.ChangeRelationshipType(newRelationshipType);

        var engagementChangeResult = searchProfile.ChangeEngagement(newEngagement);
        if (engagementChangeResult.IsError)
        {
            errors.AddRange(engagementChangeResult.Errors);
        }

        var gendersChangeResult = searchProfile.ChangeGenders(newGenders);
        if (gendersChangeResult.IsError)
        {
            errors.AddRange(gendersChangeResult.Errors);
        }

        var ageRangeChangeResult = searchProfile.ChangeAgeRange(newAgeRangeMin, newAgeRangeMax);
        if (ageRangeChangeResult.IsError)
        {
            errors.AddRange(ageRangeChangeResult.Errors);
        }

        searchProfile.ChangeMaximumDistance(newMaximumDistance);

        if (newActive != searchProfile.Active)
        {
            searchProfile.ToggleActive();
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        if (descriptionIsDifferent)
        {
            _domainEventStore.AddDomainEvent(new SearchProfileDescriptionChangedDomainEvent(searchProfile.Id, searchProfile.Description));
        }

        _domainEventStore.AddDomainEvent(new SearchProfileUpdatedDomainEvent(searchProfile));
        return searchProfile;
    }
}