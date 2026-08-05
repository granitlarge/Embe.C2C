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
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: false,
        includeBio: false,
        includeImages: false,
        includeSettings: false
    );

    public static readonly UserVariant Blocked = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: false,
        includeGender: false,
        includeLocation: false,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: false,
        includeBio: false,
        includeImages: false,
        includeSettings: false
    );

    public static readonly UserVariant Matched = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        includeLocation: false,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: true,
        includeBio: true,
        includeImages: true,
        includeSettings: false
    );

    public static readonly UserVariant Candidate = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        includeLocation: false,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: true,
        includeBio: true,
        includeImages: true,
        includeSettings: false
    );

    public static readonly UserVariant PositivelyJudged = new
    (
        includeEmail: false,
        includeUserName: true,
        includeBirthDate: false,
        includeAge: true,
        includeGender: true,
        includeLocation: false,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeDistance: true,
        includeBio: true,
        includeImages: true,
        includeSettings: false
    );

    public static readonly UserVariant Full = new
    (
        includeEmail: true,
        includeUserName: true,
        includeBirthDate: true,
        includeAge: true,
        includeGender: true,
        includeLocation: true,
        includeCreatedAt: true,
        includeUpdatedAt: true,
        includeDistance: true,
        includeBio: true,
        includeImages: true,
        includeSettings: true
    );

    public UserVariant
    (
        bool includeEmail,
        bool includeUserName,
        bool includeBirthDate,
        bool includeAge,
        bool includeGender,
        bool includeLocation,
        bool includeCreatedAt,
        bool includeUpdatedAt,
        bool includeDistance,
        bool includeBio,
        bool includeImages,
        bool includeSettings
    )
    {
        IncludeEmail = includeEmail;
        IncludeAlias = includeUserName;
        IncludeBirthDate = includeBirthDate;
        IncludeAge = includeAge;
        IncludeGender = includeGender;
        IncludeLocation = includeLocation;
        IncludeCreatedAt = includeCreatedAt;
        IncludeUpdatedAt = includeUpdatedAt;
        IncludeDistanceToQueryingUser = includeDistance;
        IncludeBio = includeBio;
        IncludeImages = includeImages;
        IncludeSettings = includeSettings;
    }

    public bool IncludeEmail { get; }
    public bool IncludeAlias { get; }
    public bool IncludeBirthDate { get; }
    public bool IncludeAge { get; }
    public bool IncludeGender { get; }
    public bool IncludeLocation { get; }
    public bool IncludeCreatedAt { get; }
    public bool IncludeUpdatedAt { get; }
    public bool IncludeDistanceToQueryingUser { get; }
    public bool IncludeBio { get; }
    public bool IncludeImages { get; }
    public bool IncludeSettings { get; }
}