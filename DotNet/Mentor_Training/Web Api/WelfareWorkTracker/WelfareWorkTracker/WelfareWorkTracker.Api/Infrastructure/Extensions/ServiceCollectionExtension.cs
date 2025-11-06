using MassTransit;
using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Api.Configurations;
using WelfareWorkTracker.Api.Infrastructure.Handler;
using WelfareWorkTracker.Core.Constants;
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
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
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
