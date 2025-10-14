namespace JwtAuthentication.Api.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
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
        services.AddScoped<JwtService>();

        services.AddTransient<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        var ConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<JwtContext>(options => options.UseSqlServer(ConnectionString));
    }

    public static void RegisterConfigurationServices(IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["JWT:Key"]!;
        var jwtIssuer = configuration["JWT:Issuer"]!;
        var jwtAudience = configuration["JWT:Audience"]!;

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
