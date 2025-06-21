namespace Saga.Orchestrator.SagaOrderManager;

public enum OrderAction
{
    GetBasket,
    CreateOrder,
    GetOrder,
    UpdateInventory,
    RollbackInventory
}
