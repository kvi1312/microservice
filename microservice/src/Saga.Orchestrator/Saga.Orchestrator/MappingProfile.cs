using AutoMapper;
using Shared.DTOS.Basket;
using Shared.DTOS.Inventory;
using Shared.DTOS.Order;

namespace Saga.Orchestrator;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BasketCheckoutDto, CreateOrderDto>();
        CreateMap<CartItemDto, SaleItemDto>();
    }
}