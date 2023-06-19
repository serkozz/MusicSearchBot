public class ErrorInfo
{
    public ErrorCode ErrorCode { get; }
    public object? Details { get; }
    public bool ShowErrorToUser { get; }
    public string Message { get; }
    public ErrorInfo(ErrorCode errorCode, string message, bool showErrorToUser = false, object? details = null)
    {
        ErrorCode = errorCode;
        Details = details;
        ShowErrorToUser = showErrorToUser;
        Message = message;
    }

    public override string ToString()
    {
        if (ShowErrorToUser == true)
            return $"Возникла ошибка: {Message}";
        return $"Ошибка --- ErrorCode: {ErrorCode}, ShowToUser: {ShowErrorToUser}, Message: {Message}, Details: {Details?.ToString()}";
    }
}

public enum ErrorCode
{
    ParseError
}