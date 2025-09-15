namespace StudentManagement.Infrastructure.Services;

public class CourseService: ICourseService
{
    public static List<Course> courseData = new List<Course>();
    public void AddCourse()
    {
        try
        {
            Console.WriteLine("Enter Course Id:");
            int courseId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Course Name:");
            string courseName = Console.ReadLine();
            Console.WriteLine("Enter Course Description:");
            string courseDescription = Console.ReadLine();
            var newCourse = new Course()
            {
                Id = courseId,
                Name = courseName,
                Description = courseDescription
            };
            var resultCourse = courseData.FirstOrDefault(course => course.Id == newCourse.Id);
            if (resultCourse != null)
            {
                throw new CourseAlreadyExistsException();
            }
            else
            {
                courseData.Add(newCourse);
                Console.WriteLine("Course Added Successfully");
            }
        }
        catch (FormatException fe)
        {
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void DeleteCourse()
    {
        try
        {
            Console.WriteLine("Enter Course Id to delete:");
            int deleteCourseId = Convert.ToInt32(Console.ReadLine());

            var courseToDelete = courseData.FirstOrDefault(c => c.Id == deleteCourseId);
            if (courseToDelete == null)
            {
                throw new CourseNotFoundException();
            }

            var isStudentEnrolled = StudentService.studentData.Any(s => s.Courses != null && s.Courses.Any(c => c.Id == deleteCourseId));
            if (isStudentEnrolled)
            {
                Console.WriteLine("Cannot delete course. One or more students are registered with the course.");
            }
            else
            {
                courseData.Remove(courseToDelete);
                Console.WriteLine("Course Deleted Successfully");
            }
        }
        catch (FormatException fe)
        {
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void GetAllCourses()
    {
        var courses = courseData;
        if (courses.Count == 0)
        {
            Console.WriteLine("No Courses Available");
        }
        else
        {
            foreach (Course course in courses)
            {
                Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
            }
        }
    }

    public void GetCourse()
    {
        try
        {
            Console.WriteLine("Enter Course Id to view:");
            var courseId = Convert.ToInt32(Console.ReadLine());
            var course = courseData.FirstOrDefault(course => course.Id == courseId);
            if (course == null)
            {
                throw new CourseNotFoundException();
            }
            else
            {
                Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
            }
        }
        catch (FormatException fe)
        {
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void UpdateCourse()
    {
        try
        {
            Console.WriteLine("Enter Course Id to update:");
            int courseId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Course Name:");
            string courseName = Console.ReadLine();
            Console.WriteLine("Enter Course Description:");
            string courseDescription = Console.ReadLine();
            var courseToUpdate = courseData.FirstOrDefault(course => course.Id == courseId);
            if (courseToUpdate == null)
            {
                throw new CourseNotFoundException();
            }
            else
            {
                courseToUpdate.Name = courseName;
                courseToUpdate.Description = courseDescription;
                Console.WriteLine("Course Updated Successfully");
            }
        }
        catch (FormatException fe)
        {
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
