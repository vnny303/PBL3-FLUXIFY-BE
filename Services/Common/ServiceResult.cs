namespace FluxifyAPI.Services.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data) => new()
        {
            Success = true,
            StatusCode = 200,
            Data = data
        };

        public static ServiceResult<T> Created(T data) => new()
        {
            Success = true,
            StatusCode = 201,
            Data = data
        };

        public static ServiceResult<T> Fail(int statusCode, string message) => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };
    }
}
