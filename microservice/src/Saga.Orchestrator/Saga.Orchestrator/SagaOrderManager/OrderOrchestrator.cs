using AutoMapper;
using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository;
using Shared.DTOS.Basket;
using Shared.DTOS.Inventory;
using Shared.DTOS.Order;
using ILogger = Serilog.ILogger;
namespace Saga.Orchestrator.SagaOrderManager;

public class OrderOrchestrator : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public OrderOrchestrator(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository,
        IInventoryHttpRepository inventoryHttpRepository, IMapper mapper, ILogger logger)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public OrderResponse CreateOrder(BasketCheckoutDto input)
    {
        var orderStateMachine =
            new Stateless.StateMachine<OrderTransactionState, OrderAction>(OrderTransactionState.NotStarted);

        long orderId = -1;
        CartDto cart = null;
        OrderDto addedOrder = null;
        string? inventoryDocumentNo = null;

        orderStateMachine.Configure(OrderTransactionState.NotStarted).PermitDynamic(OrderAction.GetBasket, () =>
        {
            cart = _basketHttpRepository.GetBasket(input.UserName).Result;
            return cart != null ? OrderTransactionState.BasketGot : OrderTransactionState.BasketGetFailed;
        });

        orderStateMachine.Configure(OrderTransactionState.BasketGot).PermitDynamic(OrderAction.CreateOrder, () =>
            {
                var order = _mapper.Map<CreateOrderDto>(input);
                order.TotalPrice = cart.TotalPrice;
                orderId = _orderHttpRepository.CreateOrder(order).Result;
                return orderId > 0 ? OrderTransactionState.OrderCreated : OrderTransactionState.OrderCreateFailed;
            })
            .OnEntry(() => orderStateMachine.Fire(OrderAction.CreateOrder));

        orderStateMachine.Configure(OrderTransactionState.OrderCreated).PermitDynamic(OrderAction.GetOrder, () =>
            {
                addedOrder = _orderHttpRepository.GetOrder(orderId).Result;
                return addedOrder != null ? OrderTransactionState.OrderGot : OrderTransactionState.OrderGetFailed;
            })
            .OnEntry(() => orderStateMachine.Fire(OrderAction.GetOrder));

        orderStateMachine.Configure(OrderTransactionState.OrderGot)
            .PermitDynamic(OrderAction.UpdateInventory, () =>
            {
                var salesOrder = new SalesOrderDto()
                {
                    OrderNo = addedOrder.DocumentNo.ToString(),
                    SaleItems = _mapper.Map<List<SaleItemDto>>(cart.Items)
                };
                inventoryDocumentNo = _inventoryHttpRepository
                    .CreateSalesOrder(addedOrder.DocumentNo.ToString(), salesOrder).Result;
                return inventoryDocumentNo != null
                    ? OrderTransactionState.InventoryUpdated
                    : OrderTransactionState.InventoryUpdatedFailed;
            })
            .OnEntry(() => orderStateMachine.Fire(OrderAction.UpdateInventory));

        orderStateMachine.Configure(OrderTransactionState.InventoryUpdated).PermitDynamic(OrderAction.DeleteBasket, () =>
        {
            var result = _basketHttpRepository.DeleteBasket(input.UserName).Result;
            return result ? OrderTransactionState.BasketDeleted : OrderTransactionState.InventoryUpdatedFailed;
        })
            .OnEntry(() => orderStateMachine.Fire(OrderAction.DeleteBasket));

        orderStateMachine.Configure(OrderTransactionState.InventoryUpdatedFailed).PermitDynamic(OrderAction.DeleteInventory, () =>
        {
            RollbackOrder(input.UserName, inventoryDocumentNo, orderId);
            return OrderTransactionState.InventoryRollback;
        })
            .OnEntry(() => orderStateMachine.Fire(OrderAction.DeleteInventory));

        orderStateMachine.Fire(OrderAction.GetBasket);
        return new OrderResponse(orderStateMachine.State == OrderTransactionState.BasketDeleted);
    }

    public OrderResponse RollbackOrder(string userName, string documentNo, long orderId)
    {
        var orderStateMachine = new Stateless.StateMachine<OrderTransactionState, OrderAction>(OrderTransactionState.RollbackInventory);

        orderStateMachine.Configure(OrderTransactionState.RollbackInventory).PermitDynamic(OrderAction.DeleteInventory, () =>
            {
                _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
                return OrderTransactionState.InventoryRollback;
            });

        orderStateMachine.Configure(OrderTransactionState.InventoryRollback).PermitDynamic(OrderAction.DeleteOrder, () =>
        {
            var result = _orderHttpRepository.DeleteOrder(orderId).Result;
            return result ? OrderTransactionState.OrderDeleted : OrderTransactionState.OrderDeleteFailed;
        })
            .OnEntry(() => orderStateMachine.Fire(OrderAction.DeleteOrder));

        orderStateMachine.Fire(OrderAction.DeleteInventory);

        return new OrderResponse(orderStateMachine.State == OrderTransactionState.InventoryRollback);
    }
}