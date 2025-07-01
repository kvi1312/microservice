using Grpc.Core;
using Inventory.GRPC.Client;
using Polly;
using Polly.Retry;
using ILogger = Serilog.ILogger;
namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _stockProtoService;
    private readonly ILogger _logger;
    private readonly AsyncRetryPolicy<StockModel> _retryPolicy;

    public StockItemGrpcService(StockProtoService.StockProtoServiceClient grpcClient, ILogger logger)
    {
        _stockProtoService = grpcClient ?? throw new ArgumentNullException(nameof(StockProtoService));
        _logger = logger;
        _retryPolicy = Policy<StockModel>.Handle<RpcException>().RetryAsync(3);
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            _logger.Information($"[BEGIN]: Get stock StockItemGrpcService ItemNo: {itemNo}");
            var stockItemRequest = new GetStockRequest { ItemNo = itemNo };
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var result = await _stockProtoService.GetStockAsync(stockItemRequest);
                if(result is not null)
                    _logger.Information($"[END]: Get stock StockItemGrpcService ItemNo: {itemNo} - Stock value : {result.Quantity}");
                return result;
            });
        }
        catch (Exception ex)
        {
            _logger.Error($"[BEGIN]: Grpc Get stock failed ItemNo: {ex.Message}");
            return new StockModel
            {
                Quantity = -1
            };
        }
    }
}
