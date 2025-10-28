using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Api.Configurations;
using WelfareWorkTracker.Api.Infrastructure.Handler;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Infrastructure.Data;
using WelfareWorkTracker.Infrastructure.Repository;
using WelfareWorkTracker.Infrastructure.Service;

namespace WelfareWorkTracker.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterSystemServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IClaimsService, ClaimsService>();
            services.AddTransient<IConstituencyService, ConstituencyService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserService, UserService>();

            services.AddScoped<IConstituencyRepository, ConstituencyRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

        }

        public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WelfareWorkTrackerContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void RegisterConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var mapper = AutoMapperConfiguration.InitializeMapper();
            services.AddSingleton(mapper);

            services.AddHttpContextAccessor();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
        }
    }
}
