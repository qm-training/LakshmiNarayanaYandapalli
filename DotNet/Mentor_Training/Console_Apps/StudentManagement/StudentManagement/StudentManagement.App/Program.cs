using Microsoft.Extensions.Logging;

namespace StudentManagement;

public class Program
{
    public static void Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });

        services.AddTransient<IStudentService, StudentService>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddSingleton<IServiceMapper, ServiceMapper>();

        IServiceProvider provider = services.BuildServiceProvider();

        var logger = provider.GetService<ILogger<Program>>();

        var serviceMapper = provider.GetRequiredService<IServiceMapper>();

        logger.LogInformation("Application Started");

        Console.WriteLine("Welcome to Student Management System");
        int choice = 0;


        do
        {
            Console.WriteLine("Select the following services:");
            Console.WriteLine("1. Student Services");
            Console.WriteLine("2. Course Services");
            Console.WriteLine("3. Exit");

            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    logger.LogInformation("User selected student service");
                    serviceMapper.StudentServiceMapper();
                    break;

                case 2:
                    logger.LogInformation("User selected course service");
                    serviceMapper.CourseServiceMapper();
                    break;

                case 3:
                    logger.LogInformation("Application stopped");
                    Console.WriteLine("Thank you for using Student Management System");
                    break;

                default:
                    logger.LogInformation("Invalid Choice: {choice}", choice);
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        } while (choice != 3);
    }
}