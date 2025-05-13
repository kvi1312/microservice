using Grpc.Core;
using Inventory.GRPC.Protos;
using Inventory.GRPC.Repositories.Interfaces;

namespace Inventory.GRPC.Services;

public class InventoryService: StockProtoService.StockProtoServiceBase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger _logger;

    public InventoryService(IInventoryRepository inventoryRepository, ILogger logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"BEGIN get stock of itemNo: {request.ItemNo}");
        var stockQuantity = await _inventoryRepository.GetStockQuantity(request.ItemNo);
        var result = new StockModel
        {
            Quantity = stockQuantity,
        };

        _logger.LogInformation($"END get stock of itemNo count : {result}");
        return result;
    }
}
