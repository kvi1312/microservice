using AutoMapper;
using Shared.DTOS.Basket;
using Shared.DTOS.Order;

namespace Saga.Orchestrator;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BasketCheckoutDto, CreateOrderDto>();
    }
}