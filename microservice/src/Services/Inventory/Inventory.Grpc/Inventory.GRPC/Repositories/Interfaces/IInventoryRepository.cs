using Contracts.Domains.Interfaces;
using Inventory.GRPC.Entities;

namespace Inventory.GRPC.Repositories.Interfaces;

public interface IInventoryRepository : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<int> GetStockQuantity(string itemNo);
}
