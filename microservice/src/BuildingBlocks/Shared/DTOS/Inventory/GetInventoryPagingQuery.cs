using Shared.SeedWork;

namespace Shared.DTOS.Inventory;

public class GetInventoryPagingQuery : PagingRequestParameter
{
    private string _itemNo;
    public string? SearchTerm { get; set; }
    public void SetItemNo(string itemNo) => _itemNo = itemNo;
    public string ItemNo() => _itemNo;
}
