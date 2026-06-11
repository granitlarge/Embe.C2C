namespace Embe.C2C.Application.Queries.Matchings;

public record GetMatchingsQuery(int Page, int Size) : PagedQuery(Page, Size);