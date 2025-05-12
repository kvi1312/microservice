using Contracts.Domains.Interfaces;
using Infrastructure.Common.Models;
using Inventory.API.Entities;
using Shared.DTOS.Inventory;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryRepository : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
    Task<PageList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query);
    Task<InventoryEntryDto> GetByIdAsync(string id);
    Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto dto);
}
