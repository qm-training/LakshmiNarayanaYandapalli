namespace StudentManagement.Infrastructure.Services;

public class StudentService: IStudentService
{
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
                throw new StudentNotFoundException();
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
                    throw new CourseNotFoundException();
                }
                studentData.FirstOrDefault(st => st.Id == studentId).Courses.AddRange(matchedCourses);

                Console.WriteLine("Courses added to student successfully");
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

            if (studentExists != null)
            {
                throw new StudentAlreadyExistsException();
            }
            else
            {
                studentData.Add(newStudent);
                Console.WriteLine("Student Added Successfully");
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

    public void DeleteStudent()
    {
        try
        {
            Console.WriteLine("Enter Student Id to delete:");
            int deleteId = Convert.ToInt32(Console.ReadLine());
            var resultStudent = studentData.FirstOrDefault(st => st.Id == deleteId);
            if (resultStudent == null)
            {
                throw new StudentNotFoundException();
            }
            else
            {
                studentData.Remove(resultStudent);
                Console.WriteLine("Student Deleted Successfully");
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

    public void GetAllStudents()
    {

        var students = studentData;
        if (students.Count == 0)
        {
            Console.WriteLine("No students available");
        }
        foreach (Student student in students)
        {
            Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
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
                foreach (Course course in courses)
                {
                    Console.WriteLine($"Id: {course.Id}, Name: {course.Name}, Description: {course.Description}");
                }
            }
            else
            {
                Console.WriteLine("No courses found for given student Id");
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

    public void GetStudentById()
    {
        try
        {
            Console.WriteLine("Enter Student Id to view:");
            var studentId = Convert.ToInt32(Console.ReadLine());
            var student = studentData.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
            {
                throw new StudentNotFoundException();
            }
            else
            {
                Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
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

    public void GetStudentsByAge()
    {
        try
        {
            Console.WriteLine("Enter Age to view students:");
            var studentAge = Convert.ToInt32(Console.ReadLine());
            var students = studentData.Where(s => s.Age == studentAge).ToList();
            if (students.Count > 0)
            {
                foreach (Student student in students)
                {
                    Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
                }
            }
            else
            {
                Console.WriteLine("No students found with given age");
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

    public void GetStudentsByCourseName()
    {
        Console.WriteLine("Enter Course Name to view students:");
        var courseName = Console.ReadLine();
        var students = studentData.Where(s => s.Courses != null && s.Courses.Any(c => c.Name.Equals(courseName))).ToList();
        if (students.Count > 0)
        {
            foreach (Student student in students)
            {
                Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, Age: {student.Age}, M1 Marks: {student.M1Marks}, M2 Marks: {student.M2Marks}, M3 Marks: {student.M3Marks}");
            }
        }
        else
        {
            Console.WriteLine("No students found enrolled in given course");
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

            if (resultStudent == null)
            {
                throw new StudentNotFoundException();
            }
            else
            {
                resultStudent.Name = name;
                resultStudent.Age = age;
                resultStudent.M1Marks = m1Marks;
                resultStudent.M2Marks = m2Marks;
                resultStudent.M3Marks = m3Marks;
                Console.WriteLine("Student Updated Successfully");
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
