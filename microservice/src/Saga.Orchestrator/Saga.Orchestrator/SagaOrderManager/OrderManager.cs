using AutoMapper;
using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository;
using Shared.DTOS.Basket;
using Shared.DTOS.Inventory;
using Shared.DTOS.Order;

namespace Saga.Orchestrator.SagaOrderManager;

public class SagaOrderManager : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public SagaOrderManager(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository,
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
        string? inventoryDocumentNo;

        orderStateMachine.Configure(OrderTransactionState.NotStarted).PermitDynamic(OrderAction.GetBasket, () =>
        {
            cart = _basketHttpRepository.GetBasket(input.UserName).Result;
            return cart != null ? OrderTransactionState.BasketGot : OrderTransactionState.BasketGetFailed;
        }).OnEntry(() => orderStateMachine.Fire(OrderAction.GetBasket));

        orderStateMachine.Configure(OrderTransactionState.BasketGot).PermitDynamic(OrderAction.CreateOrder, () =>
            {
                var order = _mapper.Map<CreateOrderDto>(input);
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

        return new OrderResponse(orderStateMachine.State == OrderTransactionState.InventoryUpdated);
    }

    public OrderResponse RollbackOrder(BasketCheckoutDto input)
    {
        return new OrderResponse(false);
    }
}