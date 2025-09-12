namespace StudentManagement.Infrastructure.Services
{
    public class ServiceMapper(IStudentService studentService, ICourseService courseService) : IServiceMapper
    {
        public void StudentServiceMapper()
        {
            int subChoice = 0;
            Console.WriteLine("Student Services Options");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Delete Student");
            Console.WriteLine("3. View All Students");
            Console.WriteLine("4. View Student by Id");
            Console.WriteLine("5. View Students by Age");
            Console.WriteLine("6. View Students by Course Name");
            Console.WriteLine("7. View Courses by Student Id");
            Console.WriteLine("8. Update Student");
            Console.WriteLine("9. Add Courses to Student");

            subChoice = Convert.ToInt32(Console.ReadLine());

            switch (subChoice)
            {
                case 1:
                    studentService.AddStudent();
                    break;

                case 2:
                    studentService.DeleteStudent();
                    break;

                case 3:
                    studentService.GetAllStudents();
                    break;

                case 4:
                    studentService.GetStudentById();
                    break;

                case 5:
                    studentService.GetStudentsByAge();
                    break;

                case 6:
                    studentService.GetStudentsByCourseName();
                    break;

                case 7:
                    studentService.GetCoursesbyStudentId();
                    break;
                case 8:
                    studentService.UpdateStudent();
                    break;

                case 9:
                    studentService.AddCoursesToStudent();
                    break;

                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        }

        public void CourseServiceMapper()
        {

            int courseChoice = 0;
            Console.WriteLine("Course Services Options");
            Console.WriteLine("1. Add Course");
            Console.WriteLine("2. Delete Course");
            Console.WriteLine("3. View All Courses");
            Console.WriteLine("4. View Course by Id");
            Console.WriteLine("5. Update Course");
            courseChoice = Convert.ToInt32(Console.ReadLine());

            switch (courseChoice)
            {
                case 1:
                    courseService.AddCourse();
                    break;

                case 2:
                    courseService.DeleteCourse();
                    break;

                case 3:
                    courseService.GetAllCourses();
                    break;
                case 4:

                    courseService.GetCourse();
                    break;

                case 5:
                    courseService.UpdateCourse();
                    break;

                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        }
    }
}
