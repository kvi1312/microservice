using Infrastructure.Common;
using Inventory.GRPC.Entities;
using Inventory.GRPC.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.GRPC.Repositories;

public class InventoryRepository : MongoDbRepository<InventoryEntry>, IInventoryRepository
{
    public InventoryRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
    {
    }

    public async Task<int> GetStockQuantity(string itemNo)
        => Collection.AsQueryable()
            .Where(x => x.ItemNo.Equals(itemNo))
            .Sum(x => x.Quantity);
}