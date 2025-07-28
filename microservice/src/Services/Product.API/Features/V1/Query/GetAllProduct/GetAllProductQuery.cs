using MediatR;
using Shared.DTOS;
using Shared.SeedWork;

namespace Product.API.Features.V1.Query.GetAllProduct;

public class GetAllProductQuery : IRequest<ApiResult<IEnumerable<ProductDto>>>
{
    public GetAllProductQuery()
    {
    }
}
