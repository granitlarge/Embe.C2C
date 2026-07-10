namespace Embe.C2C.Application.Queries.Users;

public record GetHasSearchProfileQuery
{
    public static readonly GetHasSearchProfileQuery Instance = new();
    private GetHasSearchProfileQuery() { }
}