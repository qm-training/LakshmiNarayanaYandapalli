using Microsoft.AspNetCore.Authentication.JwtBearer;
using WelfareTracker.Api.Configurations;
using WelfareWorkTrackerAuth.Core.Options;

namespace WelfareWorkTrackerAuth.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterSystemService(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public static void RegisterApplicationServices(this IServiceCollection services)
        {
        }

        public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
        {
        }

        public static void RegisterConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

            var mapper = AutoMapperConfiguration.InitializeMapper();
            services.AddSingleton(mapper);

            services.AddHttpContextAccessor();

            var jwtKey = configuration["JwtOptions:Key"]!;
            var jwtIssuer = configuration["JwtOptions:Issuer"]!;
            var jwtAudience = configuration["JwtOptions:Audience"]!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });
        }
    }
}
