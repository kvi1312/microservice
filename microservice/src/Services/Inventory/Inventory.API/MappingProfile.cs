﻿using AutoMapper;
using Inventory.API.Entities;
using Shared.DTOS.Inventory;

namespace Inventory.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<InventoryEntry, InventoryEntryDto>().ReverseMap();
        CreateMap<PurchaseProductDto, InventoryEntryDto>();
    }
}
