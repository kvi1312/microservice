namespace Shared.SeedWork;

public class PagedList<T> : List<T>
{
    private MetaData _metadata { get; }
    public MetaData GetMetadata => _metadata;
    public PagedList(IEnumerable<T> items, int totalItems, int pageNumber, int pageSize)
    {
        _metadata = new MetaData()
        {
            TotalItems = totalItems,
            PageSize = pageSize,
            CurrentPage = pageNumber,
        };
        AddRange(items);
    }
}