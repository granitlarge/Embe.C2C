using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Entities.SearchProfiles;
using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Domain.Errors.ValueObjects;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;
using ErrorOr;

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
        Id = Guid.CreateVersion7();
        Active = true;
        UserId = userId;
        Name = name;
        Description = description;
        RelationshipType = relationshipType;
        Engagement = engagement;
        _genders = [.. genders.Select(g => SearchProfileGender.Create(Id, g))];
        AgeRangeMin = ageRangeMin;
        AgeRangeMax = ageRangeMax;
        MaximumDistance = maximumDistance;

        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private SearchProfile()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public bool Active { get; private set; }
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

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    #region read-only navigation properties
    public User? User { get; private set; }
    public ICollection<Matching>? MatchingsUserId1 { get; private set; }
    public ICollection<Matching>? MatchingsUserId2 { get; private set; }
    #endregion

    internal ErrorOr<Success> AddGender(Gender gender)
    {
        if (_genders.Any(g => g.Gender == gender))
        {
            return SearchProfileErrors.AddGenderAlreadyExists.ToRuleErrorOr();
        }

        _genders.Add(SearchProfileGender.Create(Id, gender));
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    internal ErrorOr<Success> RemoveGender(Gender gender)
    {
        if (!_genders.Any(g => g.Gender == gender))
        {
            return SearchProfileErrors.RemoveGenderDoesNotExist.ToValidationErrorOr();
        }

        if (_genders.Count == 1)
        {
            return SearchProfileErrors.RemoveGenderExceedsMinimumCountOfOne.ToValidationErrorOr();
        }

        _genders.RemoveAll(g => g.Gender == gender);
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    internal void ToggleActive()
    {
        Active = !Active;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    internal ErrorOr<Success> ChangeEngagement(Engagement engagement)
    {
        Engagement = engagement;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    internal ErrorOr<Success> ChangeName(string name)
    {
        var nameValidation = ValidateName(name);
        if (nameValidation.IsError)
        {
            return nameValidation;
        }
        Name = name;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    internal ErrorOr<Success> ChangeDescription(string description)
    {
        var descriptionValidation = ValidateDescription(description);
        if (descriptionValidation.IsError)
        {
            return descriptionValidation;
        }
        Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    private static ErrorOr<Success> ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DomainErrors.Empty.ToValidationErrorOr();
        }
        return Result.Success;
    }

    private static ErrorOr<Success> ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return DomainErrors.Empty.ToValidationErrorOr();
        }
        return Result.Success;
    }

    private static ErrorOr<Success> ValidateGenders(ImmutableHashSet<Gender> genders)
    {
        if (genders.Count == 0)
        {
            return SearchProfileErrors.GendersEmpty.ToRuleErrorOr();
        }
        return Result.Success;
    }

    private static ErrorOr<Success> ValidateAgeRange(Age? ageRangeMin, Age? ageRangeMax)
    {
        if (ageRangeMin is not null && ageRangeMax is not null && ageRangeMin > ageRangeMax)
        {
            return SearchProfileErrors.AgeRangeInvalid.ToValidationErrorOr();
        }
        return Result.Success;
    }

    internal static ErrorOr<SearchProfile> Create
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
        var errors = new List<Error>();
        var nameValidation = ValidateName(name);
        if (nameValidation.IsError)
        {
            errors.AddRange(nameValidation.Errors);
        }

        var descriptionValidation = ValidateDescription(description);
        if (descriptionValidation.IsError)
        {
            errors.AddRange(descriptionValidation.Errors);
        }

        var gendersValidation = ValidateGenders(genders);
        if (gendersValidation.IsError)
        {
            errors.AddRange(gendersValidation.Errors);
        }

        var ageRangeValidation = ValidateAgeRange(ageRangeMin, ageRangeMax);
        if (ageRangeValidation.IsError)
        {
            errors.AddRange(ageRangeValidation.Errors);
        }

        if (errors.Count != 0)
        {
            return errors;
        }

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

    internal void ChangeRelationshipType(RelationshipType relationshipType)
    {
        RelationshipType = relationshipType;
    }

    internal ErrorOr<Success> ChangeGenders(ImmutableHashSet<Gender> genders)
    {
        var gendersValidation = ValidateGenders(genders);
        if (gendersValidation.IsError)
        {
            return gendersValidation;
        }

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
        return Result.Success;
    }

    internal ErrorOr<Success> ChangeAgeRange(Age? ageRangeMin, Age? ageRangeMax)
    {
        var ageRangeValidation = ValidateAgeRange(ageRangeMin, ageRangeMax);
        if (ageRangeValidation.IsError)
        {
            return ageRangeValidation;
        }

        AgeRangeMin = ageRangeMin;
        AgeRangeMax = ageRangeMax;
        return Result.Success;
    }

    internal void ChangeMaximumDistance(Distance? distance)
    {
        MaximumDistance = distance;
    }

    public void Remove()
    {

    }
}