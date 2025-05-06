namespace Shared.SeedWork;

public class PagingRequestParameter
{
    private const int MAX_PAGE_SIZE = 50;
    private int _pageIndex = 1;
    private int _pageSize= 10;

    // use for skiping to page N feature
    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if(value > 0)
            {
                _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
            }
        }
    }
}
