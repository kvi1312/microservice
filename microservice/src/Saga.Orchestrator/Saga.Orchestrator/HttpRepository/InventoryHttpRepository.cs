using Infrastructure.Extensions;
using Shared.DTOS.Inventory;

namespace Saga.Orchestrator.HttpRepository;

public class InventoryHttpRepository : IInventoryHttpRepository
{
    private readonly HttpClient _client;

    public InventoryHttpRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> CreateSalesOrder(SalesProductDto model)
    {
        var response = await _client.PostAsJsonAsync($"inventory/sales/{model.ItemNo}", model);
        
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Create sale order failed: {model.ItemNo}");
        
        var result = await response.ReadContentAs<InventoryEntryDto>();
        return result.DocumentNo;
    }

    public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
    {
        var response = await _client.DeleteAsync($"inventory/document-no/{documentNo}");
        
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Delete order for Document No: {documentNo} not success");
        
        var result = await response.ReadContentAs<bool>();
        return result;
    }
}
