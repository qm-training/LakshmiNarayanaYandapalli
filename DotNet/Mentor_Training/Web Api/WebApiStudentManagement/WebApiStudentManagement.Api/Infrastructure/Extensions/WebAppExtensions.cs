namespace WebApiStudentManagement.Api.Infrastructure.Extensions;

public static class WebAppExtensions
{
    public static void CreateMiddlewarePipeline(this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}
