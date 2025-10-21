using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WelfareTracker.Api.Configurations;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Options;
using WelfareTracker.Infrastructure.Data;
using WelfareTracker.Infrastructure.Repository;
using WelfareTracker.Infrastructure.Service;

namespace WelfareTracker.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterSystemServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

        }

        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IComplaintService, ComplaintService>();
            services.AddTransient<IComplaintStatusService, ComplaintStatusService>();
            services.AddTransient<IClaimsService, ClaimsService>();
            services.AddTransient<ICommentService, CommentService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConstituencyRepository, ConstituencyRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IComplaintStatusRepository, ComplaintStatusRepository>();
            services.AddScoped<IComplaintImageRepository, ComplaintImageRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
        }

        public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WelfareTrackerContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
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
