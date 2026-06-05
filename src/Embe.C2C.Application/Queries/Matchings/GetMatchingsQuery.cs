namespace Embe.C2C.Application.Queries.Matchings;

public record GetMatchingsQuery
{
    public static readonly GetMatchingsQuery Instance = new();
    private GetMatchingsQuery() { }
}