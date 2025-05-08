using Microsoft.AspNetCore.Http;
using Shared.SeedWork;
using System.Reflection;

namespace Shared.DTOS.Inventory;

public class GetInventoryPagingQuery : PagingRequestParameter
{
    private string _itemNo;
    public string? SearchTerm { get; set; }
    public void SetItemNo(string itemNo) => _itemNo = itemNo;
    public string ItemNo() => _itemNo;
}
