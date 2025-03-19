using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Domain.Models;

public class PagedList<T>
{
    public List<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public PagedList()
    {
    }

    public PagedList(List<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Max(pageSize, 1);

        var count = await source.CountAsync(cancellationToken);
        pageSize = Math.Min(pageSize, count);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(items, pageNumber, pageSize, count);
    }
}