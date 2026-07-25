using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users.Events;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;
using NetTopologySuite.Geometries;

namespace Embe.C2C.Domain.Aggregates.Users;

public class User : Aggregate
{
    private User
    (
        Email email,
        Alias alias,
        BirthDate birthDate,
        Gender? gender,
        ValueObjects.Location? location,
        ImmutableHashSet<ImageDetails>? files,
        string? bio,
        string identityUserId
    )
    {
        Id = Guid.CreateVersion7();
        IdentityUserId = identityUserId;
        Email = email;
        Alias = alias;
        BirthDate = birthDate;
        Gender = gender;
        Coordinates = location != null ? new Point(location.Longitude, location.Latitude) { SRID = 4326 } : null;
        Bio = bio;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;

        _images = [];
        if (files != null)
        {
            foreach (var file in files)
            {
                AddImage(file);
            }
        }

        AddDomainEvent(new UserCreatedEvent(this));
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public Guid Id { get; }
    public string IdentityUserId { get; }
    public Email Email { get; private set; }
    public Alias Alias { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public Age Age => Age.Create(BirthDate);
    public Gender? Gender { get; private set; }
    public Point? Coordinates { get; private set; }
    public ValueObjects.Location? Location => Coordinates != null ? ValueObjects.Location.Create(Coordinates.Y, Coordinates.X).Value : null;

    private readonly List<Entities.Image> _images;
    [NotMapped]
    public IReadOnlyCollection<Entities.Image> Images => _images;
    [NotMapped]
    public Entities.Image? ProfilePicture => _images.OrderBy(f => f.ImageDetails.Order).FirstOrDefault();

    public string? Bio { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    #region read only navigation properties
    public ICollection<Blocking>? Blocked { get; private set; }
    public ICollection<Blocking>? BlockedBy { get; private set; }
    public ICollection<Matching>? Matchings1 { get; private set; }
    public ICollection<Matching>? Matchings2 { get; private set; }
    public ICollection<SearchProfile>? SearchProfiles { get; private set; }

    // These are all the candidates where this user is the "user" (the one who is judging)
    public ICollection<Candidate>? CandidateUsers { get; private set; }

    // These are all the candidates where this user is the "candidate" (the one being judged)
    public ICollection<Candidate>? CandidateCandidates { get; private set; }
    #endregion

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateAlias(Alias alias)
    {
        Alias = alias;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public ErrorOr<Success> UpdateBirthDate( BirthDate newBirthDate)
    {
        if (Age.Create(newBirthDate) < Age.Create(18).Value)
        {
            return DomainErrors.UserAgeOutOfRange.ToValidationErrorOr();
        }

        BirthDate = newBirthDate;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    public void UpdateGender(Gender? newGender)
    {
        Gender = newGender;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateLocation(ValueObjects.Location? newLocation)
    {
        Coordinates = newLocation == null ? null : new Point(newLocation.Longitude, newLocation.Latitude) { SRID = 4326 };
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public ErrorOr<Entities.Image> AddImage(ImageDetails imageDetails)
    {
        if (_images.Count >= 10)
        {
            return DomainErrors.UserInvalidFileCount.ToValidationErrorOr();
        }

        var image = Entities.Image.Create(Id, imageDetails);
        _images.Add(image);
        UpdatedAt = DateTimeOffset.UtcNow;
        return image;
    }

    public void ChangeImageOrder(Guid imageId, int newOrder)
    {
        var image = _images.Single(f => f.Id == imageId);
        image.ChangeOrder(newOrder);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.Single(f => f.Id == imageId);
        _images.Remove(image);
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new UserImageRemovedEvent(image));
    }

    public void UpdateBio(string? newBio)
    {
        Bio = newBio;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Remove()
    {
        var imageIdsToRemove = _images.Select(f => f.Id).ToList();
        foreach (var imageId in imageIdsToRemove)
        {
            RemoveImage(imageId);
        }
    }

    public static ErrorOr<User> Register
    (
        Email email,
        Alias alias,
        BirthDate birthDate,
        Gender? gender,
        ValueObjects.Location? location,
        ImmutableHashSet<ImageDetails>? images,
        string? bio,
        string identityUserId
    )
    {
        var errors = new List<Error>();
        if (images != null && (images.Count > 10))
        {
            errors.Add(DomainErrors.UserInvalidFileCount.ToValidationErrorOr());
        }

        if (Age.Create(birthDate) < Age.Create(18).Value)
        {
            errors.Add(DomainErrors.UserAgeOutOfRange.ToValidationErrorOr());
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return new User
        (
            email,
            alias,
            birthDate,
            gender,
            location,
            images,
            bio,
            identityUserId
        );
    }
}
