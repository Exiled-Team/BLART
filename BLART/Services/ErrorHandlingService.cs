namespace BLART.Services;

public static class ErrorHandlingService
{
    public static string GetErrorMessage(ErrorCodes e, string extra = "") => $"Code {(int)e}: {e.ToString().SplitCamelCase()} {extra}".TrimEnd(' ');
}