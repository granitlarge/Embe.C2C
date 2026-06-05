using System.Collections.Immutable;
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
        Location location,
        ImmutableHashSet<FileDetails> files
    )
    {
        if (files.Count < 2 || files.Count > 10)
        {
            throw new DomainException("A user must have at least 2 files and no more than 10 files.");
        }

        if (new Age(birthDate) < new Age(18))
        {
            throw new DomainException("User must be at least 18 years old.");
        }

        Id = Guid.CreateVersion7();
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

    public Guid Id { get; }
    public Email Email { get; private set; }
    public UserName UserName { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public Age Age => new(BirthDate);
    public Gender Gender { get; private set; }
    public DatingPreferences DatingPreferences { get; private set; }
    public Location Location { get; private set; }

    private readonly List<Entities.File> _files;
    public IReadOnlyCollection<Entities.File> Files => _files.AsReadOnly();

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void UpdateEmail(Guid actorId, Email newEmail)
    {
        EnsureActorIsOwner(actorId);
        Email = newEmail;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateBirthDate(Guid actorId, BirthDate newBirthDate)
    {
        EnsureActorIsOwner(actorId);
        if (new Age(newBirthDate) < new Age(18))
        {
            throw new DomainException("User must be at least 18 years old.");
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

    public void UpdateLocation(Guid actorId, Location newLocation)
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
            throw new DomainException("A user cannot have more than 10 files.");
        }

        var file = Entities.File.Create(Id, fileDetails);
        _files.Add(file);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveFile(Guid actorId, Guid fileId)
    {
        EnsureActorIsOwner(actorId);
        var file = _files.FirstOrDefault(f => f.Id == fileId) ?? throw new DomainException("File not found.");
        if (_files.Count <= 2)
        {
            throw new DomainException("A user must have at least 2 files.");
        }

        file.MarkForDeletion();
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
            throw new DomainException("Only the user can perform this action.");
        }
    }

    public static User Register
    (
        Email email,
        BirthDate birthDate,
        Gender gender,
        DatingPreferences datingPreferences,
        Location location,
        ImmutableHashSet<FileDetails> files
    )
    {
        return new User
        (
            email,
            UserName.Create(email.Value),
            birthDate,
            gender,
            datingPreferences,
            location,
            files
        );
    }
}