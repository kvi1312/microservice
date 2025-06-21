using System.Text.Json.Serialization;

namespace Shared.SeedWork;

public class ApiResult<T>
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public T Data { get; set; }

    [JsonConstructor]
    public ApiResult(bool isSuccess, string message = null)
    {
        Message = message;
        IsSuccess = isSuccess;
    }
    
    public ApiResult(bool isSuccess, T data ,string message = null)
    {
        Message = message;
        IsSuccess = isSuccess;
        Data = data;
    }
}