using MongoDB.Driver;
using Shared.SeedWork;

namespace Infrastructure.Common.Models;

public class PageList<T> : List<T>
{
    public PageList(IEnumerable<T> items, long totalITems, int pageIndex, int pageSize)
    {
        _metaData = new MetaData
        {
            TotalItems = totalITems,
            PageSize = pageSize,
            CurrentPage = pageIndex,
        };
        AddRange(items);
    }

    private MetaData _metaData;
    public MetaData GetMetaData() => _metaData;
    public static async Task<PageList<T>> ToPageList(IMongoCollection<T> source, FilterDefinition<T> filter, int pageIndex, int pageSize)
    {
        var count = await source.Find(filter).CountDocumentsAsync();
        var items = await source.Find(filter)
            .Skip((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PageList<T>(items, count, pageIndex, pageSize);
    }
}

