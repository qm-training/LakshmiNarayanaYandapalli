namespace WelfareWorkTrackerAuth.Api.Infrastructure.Extensions
{
    public static class WebAppExtensions
    {
        public static void CreateMiddlewarePipeline(this WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
        }
    }
}