using Contracts.Domains;
using Infrastructure.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Enum.Inventory;

namespace Inventory.API.Entities;

[BsonCollection("InventoryEntries")]
public class InventoryEntry : MongoEntity
{
    [BsonElement("documentType")]
    public DocumentType DocumentType { get; set; }

    [BsonElement("documentNo")]
    public string DocumentNo { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("itemNo")]
    public string ItemNo { get; set; }

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("externalDocumentNo")]
    public string ExternalDocumentNo { get; set; } = Guid.NewGuid().ToString();

    public InventoryEntry(string id) => (Id) = id;

    public InventoryEntry()
    {
        DocumentType = DocumentType.Purchase;
        DocumentNo = Guid.NewGuid().ToString();
        ExternalDocumentNo = Guid.NewGuid().ToString();
    }
}
