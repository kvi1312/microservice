using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interface;
using Shared.DTOS;

namespace Product.API.EndPoints;

public static class ProductEndpoints
{
    public static WebApplication AddProductEndpoint(this WebApplication app)
    {
        app.MapGet("/api/products", async (IProductRepository productRepository, IMapper mapper) =>
        {
            var product = await productRepository.GetProducts();
            var result = mapper.Map<IEnumerable<ProductDto>>(product);
            return Results.Ok(result);
        });

        app.MapGet("/api/products/{id:long}", async (IProductRepository productRepository, IMapper mapper, long id) =>
        {
            var product = await productRepository.GetProduct(id);
            if (product is null) return Results.NotFound();

            var result = mapper.Map<ProductDto>(product);
            return Results.Ok(result);
        });

        app.MapPost("/api/products", async (IProductRepository productRepository, IMapper mapper, [FromBody] CreateProductDto productDto) =>
        {
            var productEntity = await productRepository.GetProductByNo(productDto.No);
            if (productEntity is not null) return Results.BadRequest($"Product No : {productDto.No} is existed");

            var product = mapper.Map<CatalogProduct>(productDto);
            await productRepository.CreateProduct(product);
            await productRepository.SaveChangesAsync();
            var result = mapper.Map<ProductDto>(product);
            return Results.Ok(result);
        });
        
        app.MapPut("/api/products/{id:long}", async (IProductRepository productRepository, IMapper mapper, long id, [FromBody] UpdateProductDto productDto) =>
        {
            var product = await productRepository.GetProduct(id);
            if (product is null) return Results.NotFound();

            var updatedProduct = mapper.Map(productDto, product);
            await productRepository.UpdateAsync(updatedProduct);
            await productRepository.SaveChangesAsync();
            var result = mapper.Map<ProductDto>(product);
            return Results.Ok(result);
        });

        app.MapDelete("/api/products/{id:long}", async (IProductRepository productRepository, IMapper mapper, [Required]long id) =>
        {
            var product = await productRepository.GetProduct(id);
            if (product is null) return Results.NotFound();

            await productRepository.DeleteProduct(id);
            await productRepository.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapGet("/api/products/get-product-by-no/{productNo}", async (IProductRepository productRepository, IMapper mapper, [Required] string productNo) =>
        {
            var product = await productRepository.GetProductByNo(productNo);
            if (product is null) return Results.NotFound();

            var result = mapper.Map<ProductDto>(product);
            return Results.Ok(result);
        });
        
        return app;
    }
}