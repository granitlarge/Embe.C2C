namespace Embe.C2C.Application.Commands.Candidates;

public record GenerateCandidatesCommand
{
    public static readonly GenerateCandidatesCommand Instance = new();
    private GenerateCandidatesCommand() { }
}