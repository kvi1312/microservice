namespace Saga.Orchestrator.SagaOrderManager;

public enum OrderTransactionState
{
    NotStarted,
    BasketGot,
    BasketGetFailed,
    BasketDeleted,
    OrderCreated,
    OrderCreateFailed,
    OrderDeleted,
    OrderDeleteFailed,
    OrderGot,
    OrderGetFailed,
    InventoryUpdated,
    InventoryUpdatedFailed,
    InventoryRollback,
    RollbackInventory,
    InventoryRollbackFailed
}