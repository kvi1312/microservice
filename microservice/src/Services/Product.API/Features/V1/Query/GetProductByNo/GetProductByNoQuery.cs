using MediatR;
using Shared.DTOS;
using Shared.SeedWork;

namespace Product.API.Features.V1.Query.GetProductByNo;

public class GetProductByNoQuery : IRequest<ApiResult<ProductDto>>
{
    public string ProductNo { get; private set; }

    public GetProductByNoQuery(string productNo)
    {
        ProductNo = productNo;
    }
}
