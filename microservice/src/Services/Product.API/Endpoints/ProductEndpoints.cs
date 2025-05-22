using System.ComponentModel.DataAnnotations;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Product.API.Services;
using Shared.DTOS;

namespace Product.API.EndPoints;
public class ProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/product", GetProducts);
        app.MapPost("api/product", AddProduct).RequireAuthorization();
        app.MapGet("api/product/{id:long}", GetProductById);
        app.MapPut("api/product/{id:long}", UpdateProduct).RequireAuthorization();
        app.MapDelete("api/product/{id:long}", RemoveProduct).RequireAuthorization();
        app.MapGet("api/product/get-product-by-no/{productNo}", GetProductByNo);
    }

    #region Delegate - Can reuse for unit test

    private async Task<IResult> GetProducts(
    IProductService productService)
        => await productService.GetAllProducts();

    private async Task<IResult> GetProductById(IProductService productService, long id)
        => await productService.GetProduct(id);

    private async Task<IResult> AddProduct(IProductService productService, [FromBody] CreateProductDto productDto)
        => await productService.AddProduct(productDto);

    private async Task<IResult> UpdateProduct(IProductService productService, long id, [FromBody] UpdateProductDto productDto)
        => await productService.UpdateProduct(id, productDto);

    private async Task<IResult> RemoveProduct(IProductService productService, [Required] long id)
        => await productService.RemoveProduct(id);

    private async Task<IResult> GetProductByNo(IProductService productService, [Required] string productNo)
        => await productService.GetProductByNo(productNo);
    #endregion
}