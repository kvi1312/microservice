using Shared.Enum.Inventory;

namespace Shared.DTOS.Inventory;

public record SalesProductDto(string externalDocumentNo, int quantity)
{
    public DocumentType DocumentType = DocumentType.Sale;
    public string ItemNo { get; set; } = string.Empty;
    public void SetItemNo(string itemNo)
    {
        ItemNo = itemNo;
    }
}
