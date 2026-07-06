namespace Embe.C2C.Application.Queries.Users;

public record GetMeQuery
{
    public static readonly GetMeQuery Instance = new();
    private GetMeQuery() { }
}