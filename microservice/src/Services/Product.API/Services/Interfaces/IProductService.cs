using Shared.DTOS;

namespace Product.API.Services;

public interface IProductService
{
    Task<IResult> AddProduct(CreateProductDto productDto);
    Task<IResult> UpdateProduct(long id, UpdateProductDto productDto);
    Task<IResult> RemoveProduct(long id);
}