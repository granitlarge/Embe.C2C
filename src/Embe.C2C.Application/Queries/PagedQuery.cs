namespace Embe.C2C.Application.Queries;

public record PagedQuery
{
    public PagedQuery(int page, int size)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than 0.");
        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size), "Page size must be greater than 0.");

        Page = page;
        Size = size;
    }

    public int Page { get; init; }
    public int Size { get; init; }
}

public record PagedQuery<T>(T Filter, int Page, int Size) : PagedQuery(Page, Size);