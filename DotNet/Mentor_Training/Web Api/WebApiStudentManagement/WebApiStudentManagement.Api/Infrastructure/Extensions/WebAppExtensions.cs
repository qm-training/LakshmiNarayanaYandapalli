namespace WebApiStudentManagement.Api.Infrastructure.Extensions;

public static class WebAppExtensions
{
    public static void CreateMiddlewarePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}
