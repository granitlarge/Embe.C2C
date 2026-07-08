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
        includeLocation: false,
        includeProfilePicture: false,
        includeFiles: false,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: false
    );

    public static readonly UserVariant Blocked = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: false,
        includeGender: false,
        includeLocation: false,
        includeProfilePicture: false,
        includeFiles: false,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: false
    );

    public static readonly UserVariant Matched = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        includeLocation: false,
        includeProfilePicture: true,
        includeFiles: true,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: true
    );

    public static readonly UserVariant Candidate = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        includeLocation: false,
        includeProfilePicture: true,
        includeFiles: true,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: true
    );

    public static readonly UserVariant PositivelyJudged = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        includeLocation: false,
        includeProfilePicture: true,
        includeFiles: true,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: true
    );

    public static readonly UserVariant Full = new
    (
        includeEmail: true,
        includeUserName: true,
        includeBirthDate: true,
        includeAge: true,
        includeGender: true,
        includeLocation: true,
        includeProfilePicture: true,
        includeFiles: true,
        includeCreatedAt: true,
        includeUpdatedAt: true,
        includeDistance: true
    );

    public UserVariant
    (
        bool includeEmail,
        bool includeUserName,
        bool includeBirthDate,
        bool includeAge,
        bool includeGender,
        bool includeLocation,
        bool includeProfilePicture,
        bool includeFiles,
        bool includeCreatedAt,
        bool includeUpdatedAt,
        bool includeDistance
    )
    {
        IncludeEmail = includeEmail;
        IncludeAlias = includeUserName;
        IncludeBirthDate = includeBirthDate;
        IncludeAge = includeAge;
        IncludeGender = includeGender;
        IncludeLocation = includeLocation;
        IncludeProfilePicture = includeProfilePicture;
        IncludeImages = includeFiles;
        IncludeCreatedAt = includeCreatedAt;
        IncludeUpdatedAt = includeUpdatedAt;
        IncludeDistance = includeDistance;
    }

    public bool IncludeEmail { get; }
    public bool IncludeAlias { get; }
    public bool IncludeBirthDate { get; }
    public bool IncludeAge { get; }
    public bool IncludeGender { get; }
    public bool IncludeLocation { get; }
    public bool IncludeProfilePicture { get; }
    public bool IncludeImages { get; }
    public bool IncludeCreatedAt { get; }
    public bool IncludeUpdatedAt { get; }
    public bool IncludeDistance { get; }
}