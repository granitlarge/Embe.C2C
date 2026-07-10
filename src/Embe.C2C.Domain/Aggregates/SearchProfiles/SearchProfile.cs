using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Entities.SearchProfiles;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;

namespace Embe.C2C.Domain.Aggregates.SearchProfiles;

public class SearchProfile : Aggregate
{
    private SearchProfile
    (
        Guid userId,
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
        ValidateName(name);
        ValidateDescription(description);
        ValidateGenders(genders);
        ValidateAgeRange(ageRangeMin, ageRangeMax);

        Id = Guid.CreateVersion7();
        UserId = userId;
        Name = name;
        Description = description;
        RelationshipType = relationshipType;
        Engagement = engagement;
        _genders = [.. genders.Select(g => SearchProfileGender.Create(Id, g))];
        AgeRangeMin = ageRangeMin;
        AgeRangeMax = ageRangeMax;
        MaximumDistance = maximumDistance;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private SearchProfile()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public RelationshipType RelationshipType { get; private set; }
    public Engagement Engagement { get; private set; }

    private readonly List<SearchProfileGender> _genders;
    [NotMapped]
    public IReadOnlyCollection<SearchProfileGender> Genders => _genders.AsReadOnly();

    public Age? AgeRangeMin { get; private set; }
    public Age? AgeRangeMax { get; private set; }
    public Distance? MaximumDistance { get; private set; }

    #region read-only navigation properties
    public User? User { get; private set; }
    public ICollection<Matching>? MatchingsUserId1 { get; private set; }
    public ICollection<Matching>? MatchingsUserId2 { get; private set; }
    #endregion

    public void AddGender(Gender gender)
    {
        if (_genders.Any(g => g.Gender == gender))
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.InvalidGenders));
        }

        _genders.Add(SearchProfileGender.Create(Id, gender));
    }

    public void RemoveGender(Gender gender)
    {
        if (!_genders.Any(g => g.Gender == gender))
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.InvalidGenders));
        }

        if (_genders.Count == 1)
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.InvalidGenders));
        }

        _genders.RemoveAll(g => g.Gender == gender);
    }

    public void ChangeEngagement(Engagement engagement)
    {
        Engagement = engagement;
    }

    public void ChangeName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void ChangeDescription(string description)
    {
        ValidateDescription(description);
        Description = description;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.InvalidName));
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.InvalidDescription));
        }
    }

    private static void ValidateGenders(ImmutableHashSet<Gender> genders)
    {
        if (genders.Count == 0)
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.InvalidGenders));
        }
    }

    private static void ValidateAgeRange(Age? ageRangeMin, Age? ageRangeMax)
    {
        if (ageRangeMin is not null && ageRangeMax is not null && ageRangeMin > ageRangeMax)
        {
            throw new DomainException(new DomainError<SearchProfileDomainError>(SearchProfileDomainError.InvalidAgeRange));
        }
    }

    public static SearchProfile Create
    (
        Guid userId,
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
        return new SearchProfile
        (
            userId,
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

    public void ChangeRelationshipType(RelationshipType relationshipType)
    {
        RelationshipType = relationshipType;
    }

    public void ChangeGenders(ImmutableHashSet<Gender> genders)
    {
        ValidateGenders(genders);

        var deleted = _genders.Where(g => !genders.Contains(g.Gender)).ToList();
        var added = genders.Where(g => !_genders.Any(spg => spg.Gender == g)).ToList();
        foreach (var d in deleted)
        {
            _genders.Remove(d);
        }

        foreach (var a in added)
        {
            _genders.Add(SearchProfileGender.Create(Id, a));
        }
    }

    public void ChangeAgeRange(Age? ageRangeMin, Age? ageRangeMax)
    {
        ValidateAgeRange(ageRangeMin, ageRangeMax);

        AgeRangeMin = ageRangeMin;
        AgeRangeMax = ageRangeMax;
    }

    public void ChangeMaximumDistance(Distance? distance)
    {
        MaximumDistance = distance;
    }
}

public enum SearchProfileDomainError
{
    InvalidName,
    InvalidDescription,
    FrequencyAndBoundednessCombinationInvalid,
    FixedTermRequiresStartAndEndDate,
    InvalidGenders,
    InvalidAgeRange
}