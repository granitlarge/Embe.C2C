namespace Embe.C2C.Application.Abstractions.Settings;

public interface ISettings
{
    public SiteSettings Site { get; }
    public ApplicationSettings Application { get; }
}

public record ApplicationSettings
(
    string Name
);

public record SiteSettings
(
    string Url
);