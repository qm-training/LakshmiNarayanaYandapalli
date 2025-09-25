namespace StudentManagement.Infrastructure.Services;

public class CourseService(ILogger<CourseService> logger): ICourseService
{
    private readonly ILogger<CourseService> _logger = logger;

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
            if (resultCourse == null)
            {
                courseData.Add(newCourse);
                _logger.LogInformation("Course with Id {CourseId} added successfully", courseId);
                Console.WriteLine("Course Added Successfully");
            }
            else
            {
                throw new StudentManagementException("Course Already Exists");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format while adding a course");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in AddCourse");
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
                throw new StudentManagementException("Course Not Found");
            }

            var isStudentEnrolled = StudentService.studentData.Any(s => s.Courses != null && s.Courses.Any(c => c.Id == deleteCourseId));
            if (isStudentEnrolled)
            {
                _logger.LogWarning("Attempt to delete CourseId {CourseId} failed: students are still enrolled", deleteCourseId);
                Console.WriteLine("Cannot delete course. One or more students are registered with the course.");
            }
            else
            {
                courseData.Remove(courseToDelete);
                _logger.LogInformation("Course with Id {CourseId} deleted successfully", deleteCourseId);
                Console.WriteLine("Course Deleted Successfully");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format while deleting a course");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in DeleteCourse");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void GetAllCourses()
    {
        var courses = courseData;
        if (courses.Count == 0)
        {
            _logger.LogInformation("No courses available in the system");
            Console.WriteLine("No courses available");
        }
        else
        {
            _logger.LogInformation("Retrieved {Count} courses", courses.Count);
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
            if (course != null)
            {
                _logger.LogInformation("Fetched details for CourseId {CourseId}", courseId);
                Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
            }
            else
            {
                throw new StudentManagementException("Course Not Found");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format while fetching course");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in GetCourse");
            Console.WriteLine($"An error occurred: {ex.Message}");
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
                throw new StudentManagementException("Course Not Found");
            }
            else
            {
                courseToUpdate.Name = courseName;
                courseToUpdate.Description = courseDescription;
                _logger.LogInformation("Course with Id {CourseId} updated successfully", courseId);
                Console.WriteLine("Course Updated Successfully");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format while updating course");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in UpdateCourse");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
