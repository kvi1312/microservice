using Shared.Enum.Inventory;

namespace Shared.DTOS.Inventory;

public class SaleItemDto
{
    public string ItemNo { get; set; }
    public int Quantity { get; set; }
    public DocumentType DocumentType => DocumentType.Sale;
}