namespace StudentManagement;

public class Program
{
    public static void Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddTransient<IStudentService, StudentService>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddSingleton<IServiceMapper, ServiceMapper>();

        IServiceProvider provider = services.BuildServiceProvider();

        var serviceMapper = provider.GetRequiredService<IServiceMapper>();

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
                    serviceMapper.StudentServiceMapper();
                    break;

                case 2:
                    serviceMapper.CourseServiceMapper();
                    break;

                case 3:
                    Console.WriteLine("Thank you for using Student Management System");
                    break;

                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        } while (choice != 3);
    }
}