namespace Embe.C2C.Application.Queries;

public record PagedQuery
{
    public PagedQuery(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than 0.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than 0.");

        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}

public record PagedQuery<T>(int PageNumber, int PageSize) : PagedQuery(PageNumber, PageSize)
{
    public T? Filter { get; init; }
}