using Shared.Enum.Inventory;
using System.Reflection.Metadata;

namespace Shared.DTOS.Inventory;

public class PurchaseProductDto
{
    public DocumentType DocumentType => DocumentType.PurchaseInternal;
    public string ItemNo { get; set; }
    public string DocumentNo { get; set; }
    public string ExternalDocumentNo { get; set; }
    public int Quantity { get; set; }
}
