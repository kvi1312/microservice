using Shared.DTOS;

namespace Product.API.Services;

public interface IProductService
{
    Task<IResult> GetAllProducts();
    Task<IResult> GetProduct(long id);
    Task<IResult> AddProduct(CreateProductDto productDto);
    Task<IResult> UpdateProduct(long id, UpdateProductDto productDto);
    Task<IResult> RemoveProduct(long id);
    Task<IResult> GetProductByNo(string productNo);
}