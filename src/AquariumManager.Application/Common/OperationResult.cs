namespace AquariumManager.Application.Common;

public class OperationResult
{
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public static OperationResult Ok() => new OperationResult { Success = true };

    public static OperationResult Fail(string errorMessage) =>
        new OperationResult { Success = false, ErrorMessage = errorMessage };
}