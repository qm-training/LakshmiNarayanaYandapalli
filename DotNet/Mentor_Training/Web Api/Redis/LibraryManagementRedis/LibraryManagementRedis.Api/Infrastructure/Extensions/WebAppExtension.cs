namespace LibraryManagementRedis.Api.Infrastructure.Extensions;
public static class WebAppExtension
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
