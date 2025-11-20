namespace WelfareWorkTrackerAuth.Api.Infrastructure.Extensions;
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
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IEmailProcessorService, EmailProcessorService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEventPublisher, EventPublisher>();

        services.AddScoped<IConstituencyRepository, ConstituencyRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IEmailPlaceholderRepository, EmailPlaceholderRepository>();
        services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
    }

    public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WelfareWorkTrackerContext>(options =>
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

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }
}