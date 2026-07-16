using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users.Events;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
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
        if (files != null && (files.Count > 10))
        {
            throw new DomainException(new DomainError<UserError>(UserError.InvalidFileCount));
        }

        if (new Age(birthDate) < new Age(18))
        {
            throw new DomainException(new DomainError<UserError>(UserError.Underage));
        }

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
                AddImage(Id, file);
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
    public Age Age => new(BirthDate);
    public Gender? Gender { get; private set; }
    public Point? Coordinates { get; private set; }
    public ValueObjects.Location? Location => Coordinates != null ? new(Coordinates.Y, Coordinates.X) : null;

    private readonly List<Entities.Image> _images;
    [NotMapped]
    public IReadOnlyCollection<Entities.Image> Images => _images.AsReadOnly();
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

    public void UpdateEmail(Guid actorId, Email newEmail)
    {
        EnsureActorIsOwner(actorId);
        Email = newEmail;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateAlias(Guid actorId, Alias alias)
    {
        EnsureActorIsOwner(actorId);
        Alias = alias;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateBirthDate(Guid actorId, BirthDate newBirthDate)
    {
        EnsureActorIsOwner(actorId);
        if (new Age(newBirthDate) < new Age(18))
        {
            throw new DomainException(new DomainError<UserError>(UserError.Underage));
        }

        BirthDate = newBirthDate;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateGender(Guid actorId, Gender? newGender)
    {
        EnsureActorIsOwner(actorId);
        Gender = newGender;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateLocation(Guid actorId, ValueObjects.Location? newLocation)
    {
        EnsureActorIsOwner(actorId);
        Coordinates = newLocation == null ? null : new Point(newLocation.Longitude, newLocation.Latitude) { SRID = 4326 };
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Entities.Image AddImage(Guid actorId, ImageDetails imageDetails)
    {
        EnsureActorIsOwner(actorId);
        if (_images.Count >= 10)
        {
            throw new DomainException(new DomainError<UserError>(UserError.InvalidFileCount));
        }

        var image = Entities.Image.Create(Id, imageDetails);
        _images.Add(image);
        return image;
    }

    public void ChangeImageOrder(Guid actorId, Guid imageId, int newOrder)
    {
        EnsureActorIsOwner(actorId);
        var image = _images.Single(f => f.Id == imageId);
        image.ChangeOrder(newOrder);
    }

    public void ChangeImageStatus(Guid actorId, Guid imageId, ImageStatus newStatus)
    {
        EnsureActorIsOwner(actorId);
        var image = _images.Single(i => i.Id == imageId);
        var oldStatus = image.ImageDetails.Status;
        image.ChangeStatus(newStatus);
        AddDomainEvent(new UserImageStatusChangedEvent(oldStatus, image));
    }

    public void RemoveImage(Guid actorId, Guid imageId)
    {
        EnsureActorIsOwner(actorId);
        var image = _images.Single(f => f.Id == imageId);
        _images.Remove(image);
        AddDomainEvent(new UserImageRemovedEvent(image));
    }

    public void UpdateBio(Guid actorId, string? newBio)
    {
        EnsureActorIsOwner(actorId);
        Bio = newBio;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Remove()
    {
        var imageIdsToRemove = _images.Select(f => f.Id).ToList();
        foreach (var imageId in imageIdsToRemove)
        {
            RemoveImage(Id, imageId);
        }
    }

    private void EnsureActorIsOwner(Guid actorId)
    {
        if (actorId != Id)
        {
            throw new DomainException(new DomainError<UserError>(UserError.Unauthorized));
        }
    }

    public static User Register
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

public enum UserError
{
    InvalidFileCount,
    Underage,
    Unauthorized
}