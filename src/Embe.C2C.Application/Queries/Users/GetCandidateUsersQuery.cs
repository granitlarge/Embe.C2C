namespace Embe.C2C.Application.Queries.Users;

public record GetCandidateUsersQuery
{
    public static readonly GetCandidateUsersQuery Instance = new();
    private GetCandidateUsersQuery() { }
}