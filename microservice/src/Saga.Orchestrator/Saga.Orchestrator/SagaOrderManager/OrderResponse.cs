namespace Saga.Orchestrator.SagaOrderManager;

public class OrderResponse
{
    public bool Success { get; }
    public OrderResponse(bool success)
    {
        Success = success;
    }
}