using Shared.Enum.Inventory;

namespace Shared.DTOS.Inventory;

// ExternalDocumentNo = order.documentNo
public record SalesProductDto(string ExternalDocumentNo, int Quantity)
{
    public DocumentType DocumentType = DocumentType.Sale;
    public string ItemNo { get; set; } = string.Empty;
    public void SetItemNo(string itemNo)
    {
        ItemNo = itemNo;
    }
}
