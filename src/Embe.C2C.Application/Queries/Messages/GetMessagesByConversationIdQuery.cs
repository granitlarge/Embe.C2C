namespace Embe.C2C.Application.Queries.Messages;

public record GetMessagesByMatchingIdQuery(Guid MatchingId, int Page, int Size) : PagedQuery<Guid>(MatchingId, Page, Size);