namespace StudentManagement.Infrastructure.Services;

public class CourseService: ICourseService
{
    public static List<Course> courseData = new List<Course>();
    public void AddCourse()
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
        if (resultCourse == null)
        {
            courseData.Add(newCourse);
            Console.WriteLine("Course Added Successfully");
        }
        else
        {
            Console.WriteLine("Course with given Id already exists");
        }
    }

    public void DeleteCourse()
    {
        Console.WriteLine("Enter Course Id to delete:");
        int deleteCourseId = Convert.ToInt32(Console.ReadLine());

        var courseToDelete = courseData.FirstOrDefault(c => c.Id == deleteCourseId);
        if (courseToDelete == null)
        {
            Console.WriteLine("Course with given Id does not exist");
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

    public void GetAllCourses()
    {
        var courses = courseData;
        foreach (Course course in courses)
        {
            Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
        }
    }

    public void GetCourse()
    {
        Console.WriteLine("Enter Course Id to view:");
        var courseId = Convert.ToInt32(Console.ReadLine());
        var course = courseData.FirstOrDefault(course => course.Id == courseId);
        if (course != null)
        {
            Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
        }
        else
        {
            Console.WriteLine("Course not found with given Id");
        }
    }

    public void UpdateCourse()
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
            Console.WriteLine("Course with given Id does not exist");
        }
        else
        {
            courseToUpdate.Name = courseName;
            courseToUpdate.Description = courseDescription;
            Console.WriteLine("Course Updated Successfully");
        }
    }
}
