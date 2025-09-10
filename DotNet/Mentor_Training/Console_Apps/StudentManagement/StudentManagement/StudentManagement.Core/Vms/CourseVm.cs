using StudentManagement.Core.Contracts.Services;
using StudentManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Vms
{
    public class CourseVm
    {
        private readonly ICourseService courseService;
        public void CourseOptions()
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
                    Console.WriteLine("Enter Course Id:");
                    int courseId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter Course Name:");
                    string courseName = Console.ReadLine();
                    Console.WriteLine("Enter Course Description:");
                    string courseDescription = Console.ReadLine();
                    var c = new Course()
                    {
                        Id = courseId,
                        Name = courseName,
                        Description = courseDescription
                    };
                    Console.WriteLine(courseService.AddCourse(c));
                    break;

                case 2:
                    Console.WriteLine("Enter Course Id to delete:");
                    int deleteCourseId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine(courseService.DeleteCourse(deleteCourseId));
                    break;

                case 3:
                    var courses = courseService.GetAllCourses();
                    foreach (Course course in courses)
                    {
                        Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
                    }
                    break;
                case 4:

                    Console.WriteLine("Enter Course Id to view:");
                    c = courseService.GetCourse(Convert.ToInt32(Console.ReadLine()));
                    if (c != null)
                    {
                        Console.WriteLine($"Id: {c.Id}, Name: {c.Name}, Description: {c.Description}");
                    }
                    else
                    {
                        Console.WriteLine("Course not found with given Id");
                    }
                    break;

                case 5:
                    Console.WriteLine("Enter Course Id to update:");
                    courseId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter Course Name:");
                    courseName = Console.ReadLine();
                    Console.WriteLine("Enter Course Description:");
                    courseDescription = Console.ReadLine();
                    c = new Course()
                    {
                        Id = courseId,
                        Name = courseName,
                        Description = courseDescription
                    };
                    Console.WriteLine(courseService.UpdateCourse(c));
                    break;

                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        }
    }
}
