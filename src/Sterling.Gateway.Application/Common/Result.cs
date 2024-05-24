namespace Sterling.Gateway.Application;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public string? Error { get; set; }
    public string? ResponseCode { get; set; }
    public string? ResponseDescription { get; set; }

    public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value, ResponseCode = "000", ResponseDescription = "Successful" };
    public static Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Error = error, ResponseCode = "999", ResponseDescription = error };
}
