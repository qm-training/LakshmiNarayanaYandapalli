using LibraryManagementRedis.Api.Infrastructure.Handlers;
using LibraryManagementRedis.Core.Contracts.Caching;
using LibraryManagementRedis.Core.Contracts.Repository;
using LibraryManagementRedis.Core.Contracts.Services;
using LibraryManagementRedis.Infrastructure.Caching;
using LibraryManagementRedis.Infrastructure.Data;
using LibraryManagementRedis.Infrastructure.Repository;
using LibraryManagementRedis.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace LibraryManagementRedis.Api.Infrastructure.Extensions;
public static class ServiceCollectionsExtension
{
    public static void RegisterSystemService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        RegisterDatabases(services, configuration);
        RegisterConfigurationServices(services, configuration);
    }

    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IBookService, BookService>();
        services.AddTransient<IBorrowerService, BorrowerService>();

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBorrowerRepository, BorrowerRepository>();

        services.AddScoped<ICacheService, CacheService>();
    }

    public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(config);
        });

        var ConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(ConnectionString));
    }

    public static void RegisterConfigurationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //var mapper = AutoMapperConfiguration.InitializeMapper();
        //services.AddSingleton(mapper);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

    }
}
