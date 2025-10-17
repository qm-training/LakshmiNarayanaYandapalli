namespace WelfareTracker.Core.Exceptions;
public class WelfareTrackerException : Exception
{
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }

    public WelfareTrackerException() : base()
    {
        StatusCode = 500;
        ErrorMessage = "An Unknown Error Occurred!";
    }

    public WelfareTrackerException(string message) : base(message)
    {
        StatusCode = 500;
        ErrorMessage = message;
    }

    public WelfareTrackerException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorMessage = message;
    }

    public WelfareTrackerException(string message, int statusCode, Exception? innerException) : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorMessage = message;
    }

    public WelfareTrackerException(string? message, Exception? innerException) : base(message, innerException)
    {
        StatusCode = 500;
        ErrorMessage = message ?? string.Empty;
    }
}
