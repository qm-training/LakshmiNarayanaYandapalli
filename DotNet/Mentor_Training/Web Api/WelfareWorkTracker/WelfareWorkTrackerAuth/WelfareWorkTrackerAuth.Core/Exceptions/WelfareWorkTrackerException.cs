namespace WelfareWorkTrackerAuth.Core.Exceptions;
public class WelfareWorkTrackerException : Exception
{
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }

    public WelfareWorkTrackerException() : base()
    {
        StatusCode = 500;
        ErrorMessage = "An Unknown Error Occurred!";
    }

    public WelfareWorkTrackerException(string message) : base(message)
    {
        StatusCode = 500;
        ErrorMessage = message;
    }

    public WelfareWorkTrackerException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorMessage = message;
    }

    public WelfareWorkTrackerException(string message, int statusCode, Exception? innerException) : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorMessage = message;
    }

    public WelfareWorkTrackerException(string? message, Exception? innerException) : base(message, innerException)
    {
        StatusCode = 500;
        ErrorMessage = message ?? string.Empty;
    }
}
