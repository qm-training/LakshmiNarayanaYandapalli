namespace WebApiStudentManagement.Api.Infrastructure.Handler;
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = new ProblemDetails
        {
            Title = "An unexpected error occurred",
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Status = StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = problem.Status.Value;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
