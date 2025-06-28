

using Inventory.GRPC.Client;
using ILogger = Serilog.ILogger;
namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _stockProtoService;
    private readonly ILogger _logger;
    public StockItemGrpcService(StockProtoService.StockProtoServiceClient grpcClient, ILogger logger)
    {
        _stockProtoService = grpcClient ?? throw new ArgumentNullException(nameof(StockProtoService));
        _logger = logger;
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            _logger.Information($"[BEGIN]: Get stock StockItemGrpcService ItemNo: {itemNo}");
            var stockItemRequest = new GetStockRequest { ItemNo = itemNo };
            var result = await _stockProtoService.GetStockAsync(stockItemRequest);
            _logger.Information($"[END]: Get stock StockItemGrpcService ItemNo: {itemNo} - Stock value : {result.Quantity}");
            return result;

        }
        catch (Exception ex)
        {
            _logger.Error($"[BEGIN]: Get stock failed ItemNo: {ex.Message}");
            throw;
        }
    }
}
