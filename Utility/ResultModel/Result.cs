
public class Result<T>
{
    public bool Success { get; private set; }
    public T Data { get; private set; }
    public string ErrorMessage { get; private set; }

    public Result() { }

    public static Result<T> SuccessResponse(T data)
    {
        return new Result<T> { Success = true, Data = data };
    }

    public static Result<T> ErrorResponse(string errorMessage)
    {
        return new Result<T> { Success = false, ErrorMessage = errorMessage };
    }
}