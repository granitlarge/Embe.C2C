namespace Embe.C2C.Application.Queries.SearchProfiles;

public record GetAllSearchProfilesQuery(int Page, int PageSize) : PagedQuery(Page, PageSize);