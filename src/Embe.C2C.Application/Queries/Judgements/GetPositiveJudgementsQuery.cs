namespace Embe.C2C.Application.Queries.Judgements;

public record GetPositiveJudgementsQuery(int Page, int Size) : PagedQuery(Page, Size);