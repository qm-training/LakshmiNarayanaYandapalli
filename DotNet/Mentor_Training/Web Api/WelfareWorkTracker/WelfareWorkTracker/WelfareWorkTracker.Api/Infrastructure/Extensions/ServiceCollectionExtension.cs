using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace WelfareWorkTracker.Api.Infrastructure.Extensions;
public static class ServiceCollectionExtension
{
    public static void RegisterSystemServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddMassTransit(x =>
        {
            var queueConfig = configuration.GetSection("QueueConfig").Get<QueueConfig>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(queueConfig!.Uri, h =>
                {
                    h.Username(queueConfig.UserName);
                    h.Password(queueConfig.Password);
                });
            });
        });
    }

    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IClaimsService, ClaimsService>();
        services.AddTransient<ICommentService, CommentService>();
        services.AddTransient<IComplaintImageService, ComplaintImageService>();
        services.AddTransient<IComplaintService, ComplaintService>();
        services.AddTransient<IComplaintStatusService, ComplaintStatusService>();
        services.AddTransient<IConstituencyService, ConstituencyService>();
        services.AddTransient<IDailyComplaintService, DailyComplaintService>();
        services.AddTransient<IDailyComplaintStatusService, DailyComplaintStatusService>();
        services.AddTransient<IEmailProcessorService, EmailProcessorService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailTemplateService, EmailTemplateService>();
        services.AddTransient<IEventPublisher, EventPublisher>();
        services.AddTransient<IFeedbackService, FeedbackService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IUserService, UserService>();

        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IComplaintImageRepository, ComplaintImageRepository>();
        services.AddScoped<IComplaintRepository, ComplaintRepository>();
        services.AddScoped<IComplaintStatusRepository, ComplaintStatusRepository>();
        services.AddScoped<IConstituencyRepository, ConstituencyRepository>();
        services.AddScoped<IDailyComplaintRepository, DailyComplaintRepository>();
        services.AddScoped<IDailyComplaintStatusRepository, DailyComplaintStatusRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
        services.AddScoped<IEmailPlaceholderRepository, EmailPlaceholderRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WelfareWorkTrackerContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("WelfareWorkTrackerContext")));
    }

    public static void RegisterConfigurationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        var jwtKey = configuration["Jwt:Key"];
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(o =>
       {
           o.RequireHttpsMetadata = true;
           o.SaveToken = true;
           o.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? string.Empty)),
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = false,
               NameClaimType = "Id"
           };     
       });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        var mapper = AutoMapperConfiguration.InitializeMapper();
        services.AddSingleton(mapper);

        services.AddSwaggerGen(config =>
        {
            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. \\r\\n\\r\\n Enter 'Bearer' [space] and then your token in the text input below.\\r\\n\\r\\nExample: \\\"Bearer 12345abcdef\\\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []

                }
            });
        });

        services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("WelfareWorkTrackerContext")));
        services.AddHangfireServer();
    }
}