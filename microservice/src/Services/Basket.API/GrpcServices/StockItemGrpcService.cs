

using Inventory.GRPC.Client;

namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _stockProtoService;

    public StockItemGrpcService(StockProtoService.StockProtoServiceClient grpcClient)
    {
        _stockProtoService = grpcClient ?? throw new ArgumentNullException(nameof(StockProtoService));
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            var stockItemRequest = new GetStockRequest { ItemNo = itemNo };
            return await _stockProtoService.GetStockAsync(stockItemRequest);
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            throw;
        }
    }
}
