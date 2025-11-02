namespace WelfareWorkTrackerAuth.Api.Infrastructure.Handlers;
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        var problemDetails = new ProblemDetails
        {
            Detail = $"{exception.Message}{(exception.InnerException != null ? $", InnerException: {exception.InnerException.Message}" : string.Empty)}",
        };

        if (exception is WelfareWorkTrackerException welfareWorkTrackerException)
        {
            problemDetails.Status = welfareWorkTrackerException.StatusCode;
            problemDetails.Title = GetTitleForStatusCode(welfareWorkTrackerException.StatusCode);
            httpContext.Response.StatusCode = welfareWorkTrackerException.StatusCode;
        }
        else
        {
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "An unexpected error occurred";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static string GetTitleForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            (int)HttpStatusCode.BadRequest => nameof(HttpStatusCode.BadRequest),
            (int)HttpStatusCode.NotFound => nameof(HttpStatusCode.NotFound),
            _ => "An unexpected error occurred"
        };
    }
}
