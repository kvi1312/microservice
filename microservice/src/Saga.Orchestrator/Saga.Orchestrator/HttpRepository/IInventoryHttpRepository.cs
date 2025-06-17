using Shared.DTOS.Inventory;

namespace Saga.Orchestrator.HttpRepository;

public interface IInventoryHttpRepository
{
    Task<string> CreateSalesOrder(SalesProductDto model);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}
