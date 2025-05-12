using Inventory.API.Entities;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.Enum.Inventory;

namespace Inventory.API.Persistence;

public class InventoryDbSeed
{
    public async Task SeedDataAsync(IMongoClient mongoClient, MongoDbSettings settings)
    {
        var databaseName = settings.DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);
        var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");

        if(await inventoryCollection.EstimatedDocumentCountAsync() == 0)
        {
            await inventoryCollection.InsertManyAsync(GetPreconfiguredInventoryEntries());
        }
    }

    private IEnumerable<InventoryEntry> GetPreconfiguredInventoryEntries()
    {
        return new List<InventoryEntry>
        {
            new InventoryEntry
            {
                ItemNo = "Lotus",
                Quantity = 10,
                DocumentNo = Guid.NewGuid().ToString(),
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                DocumentType = DocumentType.Purchase,
            },
            new InventoryEntry
            {
                ItemNo = "cadilac",
                Quantity = 10,
                DocumentNo = Guid.NewGuid().ToString(),
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                DocumentType = DocumentType.Purchase,
            },
        };
    }
}
