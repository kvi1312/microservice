using Carter;
using Infrastructure.Common.Models;
using Inventory.API.Services;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOS.Inventory;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Inventory.API.Endpoints
{
    public class InventoryEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/inventory/items/{itemNo}", GetAllByItemNo)
                .Produces((int)HttpStatusCode.OK, typeof(IEnumerable<InventoryEntryDto>));

            app.MapGet("api/inventory/items/{itemNo}/paging", GetAllByItemNoPaging)
                .Produces((int)HttpStatusCode.OK, typeof(PageList<InventoryEntryDto>));

            app.MapGet("api/inventory/{id}", GetInventoryById)
                .Produces((int)HttpStatusCode.OK, typeof(InventoryEntryDto));

            app.MapPost("api/inventory/purchase/{itemNo}", PurchaseItemOrder)
                .Produces((int)HttpStatusCode.OK, typeof(InventoryEntryDto));

            app.MapDelete("api/inventory/{id}", DeleteById)
                .Produces((int)HttpStatusCode.OK, typeof(NoContentResult));
        }

        private async Task<IResult> DeleteById([Required] string id, IInventoryService inventoryService)
        {
            var entity = await inventoryService.GetByIdAsync(id);

            if (entity == null) return Results.NotFound();

            await inventoryService.DeleteAsync(id);
            return Results.NoContent();
        }

        private async Task<IResult> PurchaseItemOrder([Required] string itemNo, [FromBody] PurchaseProductDto dto, IInventoryService inventoryService)
        {
            var result = await inventoryService.PurchaseItemAsync(itemNo, dto);
            return Results.Ok(result);
        }

        private async Task<IResult> GetAllByItemNo([Required] string itemNo, IInventoryService inventoryService)
        {
            var result = await inventoryService.GetAllByItemNoAsync(itemNo);
            return Results.Ok(result);
        }

        private async Task<IResult> GetAllByItemNoPaging(
            [Required] string itemNo,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            IInventoryService inventoryService, [FromQuery] string? searchTerm = null)
        {
            var query = new GetInventoryPagingQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };
            query.SetItemNo(itemNo);
            var result = await inventoryService.GetAllByItemNoPagingAsync(query);
            return Results.Ok(result);
        }

        private async Task<IResult> GetInventoryById([Required] string id, IInventoryService inventoryService)
        {
            var result = await inventoryService.GetByIdAsync(id);
            return Results.Ok(result);
        }
    }
}
