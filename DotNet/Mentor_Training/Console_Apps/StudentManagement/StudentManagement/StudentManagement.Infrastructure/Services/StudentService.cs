namespace StudentManagement.Infrastructure.Services;

public class StudentService(ILogger<StudentService> logger): IStudentService
{
    private readonly ILogger<StudentService> _logger = logger;

    public static List<Student> studentData = new List<Student>();
    public void AddCoursesToStudent()
    {
        try
        {
            Console.WriteLine("Enter Student Id to add courses:");
            int studentId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Course Ids to add (comma separated):");
            var courseIds = Console.ReadLine().Split(',').Select(id => int.Parse(id)).ToList();

            var resultStudent = studentData.FirstOrDefault(st => st.Id == studentId);
            if (resultStudent == null)
            {
                throw new StudentManagementException("Course Already Exists");
            }
            else
            {
                if (resultStudent.Courses == null)
                {
                    resultStudent.Courses = new List<Course>();
                }

                var matchedCourses = CourseService.courseData.Where(c => courseIds.Contains(c.Id)).ToList();
                if (matchedCourses.Count != courseIds.Count)
                {
                    throw new StudentManagementException("One or more Courses Not Found Exists");
                }
                studentData.FirstOrDefault(st => st.Id == studentId).Courses.AddRange(matchedCourses);
                _logger.LogInformation("Courses {CourseIds} added to StudentId {StudentId}", courseIds, studentId);
                Console.WriteLine("Courses added to student successfully");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format while adding courses to student");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in AddCoursesToStudent");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void AddStudent()
    {
        try
        {
            Console.WriteLine("Enter Student Id:");
            int id = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Student Name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Student Age:");
            int age = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Student M1 Marks:");
            double m1Marks = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Enter Student M2 Marks:");
            double m2Marks = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Enter Student M3 Marks:");
            double m3Marks = Convert.ToDouble(Console.ReadLine());

            var newStudent = new Student()
            {
                Id = id,
                Name = name,
                Age = age,
                M1Marks = m1Marks,
                M2Marks = m2Marks,
                M3Marks = m3Marks
            };

            var studentExists = studentData.Any(st => st.Id == id);

            if (!studentExists)
            {
                studentData.Add(newStudent);
                _logger.LogInformation("Student with Id {Id} added successfully", id);
                Console.WriteLine("Student Added Successfully");
            }
            else
            {
                throw new StudentManagementException("Student Already Exists");
            }
        }
        catch(FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format while adding student");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in AddStudent");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void DeleteStudent()
    {
        try
        {
            Console.WriteLine("Enter Student Id to delete:");
            int deleteId = Convert.ToInt32(Console.ReadLine());
            var resultStudent = studentData.FirstOrDefault(st => st.Id == deleteId);
            if (resultStudent == null)
            {
                throw new StudentManagementException("Student Not Found");
            }
            else
            {
                studentData.Remove(resultStudent);
                _logger.LogInformation("Student with Id {Id} deleted successfully", deleteId);
                Console.WriteLine("Student Deleted Successfully");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format while deleting student");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in DeleteStudent");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void GetAllStudents()
    {

        var students = studentData;
        if (students.Count == 0)
        {
            _logger.LogInformation("No students available in system");
            Console.WriteLine("No students available");
        }
        else
        {
            _logger.LogInformation("Retrieved {Count} students", students.Count);
            foreach (Student student in students)
            {
                Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
            }
        }
    }

    public void GetCoursesbyStudentId()
    {
        try
        {
            Console.WriteLine("Enter Student Id to view courses:");
            var studentId = Convert.ToInt32(Console.ReadLine());
            var courses = studentData.FirstOrDefault(s => s.Id == studentId).Courses;

            if (courses != null && courses.Count > 0)
            {
                _logger.LogInformation("Retrieved {Count} courses for StudentId {StudentId}", courses.Count, studentId);
                foreach (Course course in courses)
                {
                    Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
                }
            }
            else
            {
                _logger.LogWarning("No courses found for StudentId {StudentId}", studentId);
                Console.WriteLine("No courses found for given student Id");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format in GetCoursesbyStudentId");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetCoursesbyStudentId");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void GetStudentById()
    {
        try
        {
            Console.WriteLine("Enter Student Id to view:");
            var studentId = Convert.ToInt32(Console.ReadLine());
            var student = studentData.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                _logger.LogInformation("Fetched student details for Id {Id}", studentId);
                Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
            }
            else
            {
                throw new StudentManagementException("Student Not Found");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format in GetStudentById");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetStudentById");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void GetStudentsByAge()
    {
        try
        {
            Console.WriteLine("Enter Age to view students:");
            var studentAge = Convert.ToInt32(Console.ReadLine());
            var students = studentData.Where(s => s.Age == studentAge).ToList();
            if (students.Count > 0)
            {
                _logger.LogInformation("Retrieved {Count} students with Age {Age}", students.Count, studentAge);
                foreach (Student student in students)
                {
                    Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
                }
            }
            else
            {
                _logger.LogWarning("No students found with Age {Age}", studentAge);
                Console.WriteLine("No students found with given age");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format in GetStudentsByAge");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetStudentsByAge");
            Console.WriteLine($"An error occurred: {ex.Message}");

        }
    }

    public void GetStudentsByCourseName()
    {
        try
        {
            Console.WriteLine("Enter Course Name to view students:");
            var courseName = Console.ReadLine();
            var students = studentData.Where(s => s.Courses != null && s.Courses.Any(c => c.Name.Equals(courseName))).ToList();
            if (students.Count > 0)
            {
                _logger.LogInformation("Retrieved {Count} students enrolled in Course {CourseName}", students.Count, courseName);
                foreach (Student student in students)
                {
                    Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
                }
            }
            else
            {
                _logger.LogWarning("No students found enrolled in Course {CourseName}", courseName);
                Console.WriteLine("No students found enrolled in given course");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetStudentsByCourseName");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void UpdateStudent()
    {
        try
        {
            Console.WriteLine("Enter Student Id to update:");
            int id = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Student Name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Student Age:");
            int age = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Student M1 Marks:");
            double m1Marks = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Enter Student M2 Marks:");
            double m2Marks = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Enter Student M3 Marks:");
            double m3Marks = Convert.ToDouble(Console.ReadLine());

            var resultStudent = studentData.FirstOrDefault(st => st.Id == id);

            if (resultStudent != null)
            {
                resultStudent.Name = name;
                resultStudent.Age = age;
                resultStudent.M1Marks = m1Marks;
                resultStudent.M2Marks = m2Marks;
                resultStudent.M3Marks = m3Marks;

                _logger.LogInformation("Student with Id {Id} updated successfully", id);

                Console.WriteLine("Student Updated Successfully");
            }
            else
            {
                throw new StudentManagementException("Student Not Found");
            }
        }
        catch (FormatException fe)
        {
            _logger.LogError(fe, "Invalid input format in UpdateStudent");
            Console.WriteLine(fe.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in UpdateStudent");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
