namespace Saga.Orchestrator.SagaOrderManager;

public enum OrderTransactionState
{
    NotStarted,
    BasketGot,
    BasketGetFailed,
    OrderCreated,
    OrderCreateFailed,
    OrderGot,
    OrderGetFailed,
    InventoryUpdated,
    InventoryUpdatedFailed,
    InventoryRollback
}