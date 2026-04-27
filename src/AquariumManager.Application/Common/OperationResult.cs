namespace AquariumManager.Application.Common;

public class OperationResult
{
    public bool Success { get; protected set; }
    public string ErrorMessage { get; protected set; } = string.Empty;

    public static OperationResult Ok() => new OperationResult { Success = true };

    public static OperationResult Fail(string errorMessage) =>
        new OperationResult { Success = false, ErrorMessage = errorMessage };
}

public class OperationResult<T> : OperationResult
{
    public T? Data { get; private set; }

    public static OperationResult<T> Ok(T data) => new OperationResult<T> { Success = true, Data = data };

    public new static OperationResult<T> Fail(string errorMessage) =>
        new OperationResult<T> { Success = false, ErrorMessage = errorMessage };
}
