using WebApiStudentManagement.Api.Infrastructure.Handler;

namespace WebApiStudentManagement.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterSystemService(
    this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        RegisterDatabases(services, configuration);
        RegisterConfigurationServices(services, configuration);
    }

    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<IStudentService, StudentService>();
        services.AddTransient<ITeacherService, TeacherService>();

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
    }

    public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        var ConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<WebApiStudentManagementDbContext>(options => options.UseSqlServer(ConnectionString,
            b => b.MigrationsAssembly("WebApiStudentManagement.Infrastructure")));
    }

    public static void RegisterConfigurationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

    }
}
