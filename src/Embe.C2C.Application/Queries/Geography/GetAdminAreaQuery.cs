namespace Embe.C2C.Application.Queries.Geography;

public record SearchAdminAreaQuery(string? ParentId, double? Longitude, double? Latitude, int Page, int Size) : PagedQuery(Page, Size);