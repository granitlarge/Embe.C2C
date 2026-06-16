using Embe.C2C.Application.Dtos.Read.Variants.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.Variants.Aggregates;

public record UserVariant
{
    public static readonly UserVariant Empty = new
    (
        includeEmail: false,
        includeUserName: false,
        includeBirthDate: false,
        includeAge: false,
        includeGender: false,
        datingPreferencesVariant: DatingPreferencesVariant.Empty,
        includeLocation: false,
        includeProfilePicture: false,
        includeFiles: false,
        includeCreatedAt: false,
        includeUpdatedAt: false
    );

    public static readonly UserVariant Blocked = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: false,
        includeGender: false,
        datingPreferencesVariant: DatingPreferencesVariant.Empty,
        includeLocation: false,
        includeProfilePicture: false,
        includeFiles: false,
        includeCreatedAt: false,
        includeUpdatedAt: false
    );

    public static readonly UserVariant Matched = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        datingPreferencesVariant: DatingPreferencesVariant.Matched,
        includeLocation: false,
        includeProfilePicture: true,
        includeFiles: true,
        includeCreatedAt: false,
        includeUpdatedAt: false
    );

    public static readonly UserVariant Candidate = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        datingPreferencesVariant: DatingPreferencesVariant.Empty,
        includeLocation: false,
        includeProfilePicture: true,
        includeFiles: true,
        includeCreatedAt: false,
        includeUpdatedAt: false
    );

    public static readonly UserVariant Full = new
    (
        includeEmail: true,
        includeUserName: true,
        includeBirthDate: true,
        includeAge: true,
        includeGender: true,
        datingPreferencesVariant: DatingPreferencesVariant.Full,
        includeLocation: true,
        includeProfilePicture: true,
        includeFiles: true,
        includeCreatedAt: true,
        includeUpdatedAt: true
    );

    public UserVariant
    (
        bool includeEmail,
        bool includeUserName,
        bool includeBirthDate,
        bool includeAge,
        bool includeGender,
        DatingPreferencesVariant datingPreferencesVariant,
        bool includeLocation,
        bool includeProfilePicture,
        bool includeFiles,
        bool includeCreatedAt,
        bool includeUpdatedAt
    )
    {
        IncludeEmail = includeEmail;
        IncludeUserName = includeUserName;
        IncludeBirthDate = includeBirthDate;
        IncludeAge = includeAge;
        IncludeGender = includeGender;
        DatingPreferencesVariant = datingPreferencesVariant;
        IncludeLocation = includeLocation;
        IncludeProfilePicture = includeProfilePicture;
        IncludeFiles = includeFiles;
        IncludeCreatedAt = includeCreatedAt;
        IncludeUpdatedAt = includeUpdatedAt;
    }

    public bool IncludeEmail { get; }
    public bool IncludeUserName { get; }
    public bool IncludeBirthDate { get; }
    public bool IncludeAge { get; }
    public bool IncludeGender { get; }
    public DatingPreferencesVariant DatingPreferencesVariant { get; }
    public bool IncludeLocation { get; }
    public bool IncludeProfilePicture { get; }
    public bool IncludeFiles { get; }
    public bool IncludeCreatedAt { get; }
    public bool IncludeUpdatedAt { get; }
}