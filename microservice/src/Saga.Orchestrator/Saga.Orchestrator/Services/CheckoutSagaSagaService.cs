using AutoMapper;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOS.Basket;
using Shared.DTOS.Inventory;
using Shared.DTOS.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services;

public class CheckoutSagaSagaService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CheckoutSagaSagaService(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository,
        IInventoryHttpRepository inventoryHttpRepository, IMapper mapper, ILogger logger)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> CheckoutOrder(string userName, BasketCheckoutDto basketCheckoutDto)
    {
        // Get cart from basket
        _logger.Information($"[CheckoutOrder] START : Get Cart {userName}");
        var cart = await _basketHttpRepository.GetBasket(userName);

        if (cart == null) return false;

        _logger.Information($"[CheckoutOrder] END : Get Cart {userName} successfully");

        // Create order
        _logger.Information($"[CheckoutOrder] START : Create Order");
        var order = _mapper.Map<CreateOrderDto>(basketCheckoutDto);
        order.TotalPrice = cart.TotalPrice; // prevent user cheating totalprice
        var orderId = await _orderHttpRepository.CreateOrder(order);
        if (orderId < 0) return false;

        // Get order by orderId
        var addedOrder = await _orderHttpRepository.GetOrder(orderId);
        _logger.Information(
            $"[CheckoutOrder] END : Create order success, order ID: {orderId} - Document No : {addedOrder.DocumentNo}");

        var inventoryDocumentNo = new List<string>();
        bool result;
        try
        {
            // Sales item from inventory
            foreach (var item in cart.Items)
            {
                _logger.Information(
                    $"[CheckoutOrder] START : Sale Item No : {item.ItemNo} - Quantity : {item.Quantity}");
                var saleOrder = new SalesProductDto(addedOrder.DocumentNo.ToString(), item.Quantity);
                saleOrder.SetItemNo(item.ItemNo);
                var documentNo = await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
                inventoryDocumentNo.Add(documentNo);
                _logger.Information(
                    $"[CheckoutOrder] END : Sale Item No : {item.ItemNo} - Quantity : {item.Quantity} - Document No : {documentNo}");
            }

            result = await _basketHttpRepository.DeleteBasket(userName);
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            await RollbackCheckoutOrder(userName, addedOrder.Id, inventoryDocumentNo);
            result = false;
        }

        // Rollback checkout
        return result;
    }

    private async Task RollbackCheckoutOrder(string userName, long orderId, List<string> inventoryDocumentNo)
    {
        _logger.Information(
            $"[RollbackCheckoutOrder] START : RollbackCheckoutOrder for userName : {userName} - OrderId : {orderId} - Inventory document no : {string.Join(",", inventoryDocumentNo)}");

        var deletedDocumentNo = new List<string>();
        // delete order by order id
        foreach (var documentNo in inventoryDocumentNo)
        {
            await _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
            deletedDocumentNo.Add(documentNo);
        }

        _logger.Information(
            $"[RollbackCheckoutOrder] END : Delete Inventory Document No success:  {string.Join(",", deletedDocumentNo)}");
    }
}