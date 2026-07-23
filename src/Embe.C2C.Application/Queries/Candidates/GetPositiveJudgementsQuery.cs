namespace Embe.C2C.Application.Queries.Candidates;

public record GetPositiveJudgementsQuery(int Page, int PageSize) : PagedQuery(Page, PageSize);