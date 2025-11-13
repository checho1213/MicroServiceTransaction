namespace Transaction.Aplication.Common;
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string[]? Errors { get; init; }

    public static ApiResponse<T> SuccessResponse(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> ErrorResponse(params string[] errors) => new() { Success = false, Errors = errors };
}
