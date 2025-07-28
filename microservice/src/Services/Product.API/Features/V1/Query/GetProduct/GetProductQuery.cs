using MediatR;
using Shared.DTOS;
using Shared.SeedWork;

namespace Product.API.Features.V1.Query.GetProduct;

public class GetProductQuery : IRequest<ApiResult<ProductDto>>
{
    public long Id { get; private set; }

    public GetProductQuery(long id)
    {
        Id = id;
    }
}
