namespace Embe.C2C.Application.Commands.Users;

public record GenerateCandidatesCommand
{
    public static readonly GenerateCandidatesCommand Instance = new();
    private GenerateCandidatesCommand() { }
}