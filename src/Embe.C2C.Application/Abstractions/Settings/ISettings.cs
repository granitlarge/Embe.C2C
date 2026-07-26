namespace Embe.C2C.Application.Abstractions.Settings;

public interface ISettings
{
    public ApplicationSettings Application { get; }
}

public record ApplicationSettings
(
    string Name
);