using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users.Events;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Aggregates.Users;

public class User : Aggregate
{
    private User
    (
        Email email,
        UserName userName,
        BirthDate birthDate,
        Gender gender,
        DatingPreferences datingPreferences,
        Location? location,
        ImmutableHashSet<FileDetails> files,
        string identityUserId
    )
    {
        if (files.Count < 2 || files.Count > 10)
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
        UserName = userName;
        BirthDate = birthDate;
        Gender = gender;
        DatingPreferences = datingPreferences;
        Location = location;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;

        _files = [];
        foreach (var file in files)
        {
            AddFile(Id, file);
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
    public UserName UserName { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public Age Age => new(BirthDate);
    public Gender Gender { get; private set; }
    public DatingPreferences DatingPreferences { get; private set; }
    public Location? Location { get; private set; }

    private readonly List<Entities.File> _files;
    [NotMapped]
    public IReadOnlyCollection<Entities.File> Files => _files.AsReadOnly();
    [NotMapped]
    public Entities.File ProfilePicture => _files.OrderBy(f => f.FileDetails.Order).First();

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    #region read only navigation properties
    public ICollection<Blocking>? Blocked { get; private set; }
    public ICollection<Blocking>? BlockedBy { get; private set; }
    public ICollection<Matching>? Matchings1 { get; private set; }
    public ICollection<Matching>? Matchings2 { get; private set; }
    public ICollection<Judgement>? JudgementsPassed { get; private set; }
    public ICollection<Judgement>? JudgementsReceived { get; private set; }
    #endregion

    public void UpdateEmail(Guid actorId, Email newEmail)
    {
        EnsureActorIsOwner(actorId);
        Email = newEmail;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateUserName(Guid actorId, UserName userName)
    {
        EnsureActorIsOwner(actorId);
        UserName = userName;
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

    public void UpdateGender(Guid actorId, Gender newGender)
    {
        EnsureActorIsOwner(actorId);
        Gender = newGender;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdatePreferences(Guid actorId, DatingPreferences newPreferences)
    {
        EnsureActorIsOwner(actorId);
        DatingPreferences = newPreferences;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateLocation(Guid actorId, Location? newLocation)
    {
        EnsureActorIsOwner(actorId);
        Location = newLocation;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddFile(Guid actorId, FileDetails fileDetails)
    {
        EnsureActorIsOwner(actorId);
        if (_files.Count >= 10)
        {
            throw new DomainException(new DomainError<UserError>(UserError.InvalidFileCount));
        }

        var file = Entities.File.Create(Id, fileDetails);
        _files.Add(file);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeFileOrder(Guid actorId, Guid fileId, int newOrder)
    {
        EnsureActorIsOwner(actorId);
        var file = _files.First(f => f.Id == fileId);
        file.ChangeOrder(newOrder);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveFile(Guid actorId, Guid fileId)
    {
        EnsureActorIsOwner(actorId);
        var file = _files.First(f => f.Id == fileId);
        if (_files.Count <= 2)
        {
            throw new DomainException(new DomainError<UserError>(UserError.InvalidFileCount));
        }

        file.MarkForDeletion();
        file.MarkAsDeleted();
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new UserFileRemovedEvent(file));
    }

    public void Remove()
    {
        var fileIdsToRemove = _files.Select(f => f.Id).ToList();
        foreach (var fileId in fileIdsToRemove)
        {
            RemoveFile(Id, fileId);
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
        UserName userName,
        BirthDate birthDate,
        Gender gender,
        DatingPreferences datingPreferences,
        Location? location,
        ImmutableHashSet<FileDetails> files,
        string identityUserId
    )
    {
        return new User
        (
            email,
            userName,
            birthDate,
            gender,
            datingPreferences,
            location,
            files,
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