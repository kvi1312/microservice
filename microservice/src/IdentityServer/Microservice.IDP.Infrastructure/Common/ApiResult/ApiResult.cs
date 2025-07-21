using Microsoft.AspNetCore.Mvc;

namespace Microservice.IDP.Infrastructure.Common.ApiResult;

public class ApiResult<T> : IActionResult
{
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }
    public ApiResult()
    {
        
    }
    public ApiResult(bool isSuccess, T result, string? message = null)
    {
        Message = message;
        IsSuccess = isSuccess;
        Result = result;
    }

    public ApiResult(bool isSuccess, string? message = null)
    {
        Message = message;
        IsSuccess = isSuccess;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(this);
        await objectResult.ExecuteResultAsync(context);
    }
}
