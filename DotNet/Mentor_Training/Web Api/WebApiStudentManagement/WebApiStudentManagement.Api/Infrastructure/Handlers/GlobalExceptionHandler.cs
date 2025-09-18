using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApiStudentManagement.Core.Exceptions;

namespace WebApiStudentManagement.Api.Infrastructure.Handler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problem = new ProblemDetails
            {
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            switch (exception)
            {
                case StudentNotFoundException:
                    problem.Status = StatusCodes.Status404NotFound;
                    problem.Title = "Student not found";
                    break;

                case CourseNotFoundException:
                    problem.Status = StatusCodes.Status404NotFound;
                    problem.Title = "Course not found";
                    break;

                case TeacherNotFoundException:
                    problem.Status = StatusCodes.Status404NotFound;
                    problem.Title = "Teacher not found";
                    break;

                default:
                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Title = "An unexpected error occurred";
                    break;
            }

            httpContext.Response.StatusCode = problem.Status.Value;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}
