using Shared.Enum.Inventory;

namespace Shared.DTOS.Inventory;

public class InventoryEntryDto
{
    public string Id { get; set; }

    public DocumentType DocumentType { get; set; }

    public string DocumentNo { get; set; }

    public string ItemNo { get; set; }

    public int Quantity { get; set; }

    public string ExternalDocumentNo { get; set; }

}
