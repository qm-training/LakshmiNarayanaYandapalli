using StudentManagement.Core.Contracts.Services;
using StudentManagement.Core.Models;
using StudentManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services
{
    public class StudentService: IStudentService
    {
        public string AddCoursesToStudent(int studentId, List<int> courseIds)
        {
            var resultStudent = GetStudent(studentId);
            if (resultStudent == null)
            {
                return "Student does not exist with given Id";
            }
            else
            {
                if (resultStudent.Courses == null)
                {
                    resultStudent.Courses = new List<Course>();
                }
                foreach (int courseId in courseIds)
                {
                    foreach (Course c in CourseData.Courses)
                    {
                        if (c.Id == courseId)
                        {

                            resultStudent.Courses.Add(c);

                        }
                    }
                }
                return "Courses added to student successfully";
            }
        }

        public string AddStudent(Student s)
        {
            if (GetStudent(s.Id) == null)
            {
                StudentData.Students.Add(s);
                return "Student Added Successfully";
            }
            else
            {
                return "Student already exist with given student Id";
            }
        }

        public string DeleteStudent(int id)
        {
            var resultStudent = GetStudent(id);
            if (resultStudent == null)
            {
                return "Student does not exist with given Id";
            }
            else
            {
                StudentData.Students.Remove(resultStudent);
                return "Student Deleted Successfully";
            }
        }

        public List<Student> GetAllStudents()
        {
            return StudentData.Students;
        }

        public List<Course> GetCoursesbyStudentId(int id)
        {
            var resultStudent = GetStudent(id);
            if (resultStudent == null)
            {
                return null;
            }
            else
            {
                return resultStudent.Courses;
            }
        }

        public Student GetStudent(int id)
        {
            foreach (Student s in StudentData.Students)
            {
                if (s.Id == id)
                {
                    return s;
                }
            }
            return null;
        }

        public List<Student> GetStudentsByAge(int age)
        {
            var resultStudents = new List<Student>();
            foreach (Student s in StudentData.Students)
            {
                if (s.Age == age)
                {
                    resultStudents.Add(s);
                }
            }
            return resultStudents;
        }

        public List<Student> GetStudnetsByCourseName(string courseName)
        {
            var resultStudents = new List<Student>();
            foreach (Student s in StudentData.Students)
            {
                foreach (Course c in s.Courses)
                {
                    if (c.Name.Equals(courseName))
                    {
                        resultStudents.Add(s);
                        break;
                    }
                }
            }
            return resultStudents;
        }

        public string UpdateStudent(Student s)
        {
            var resultStudent = GetStudent(s.Id);

            if (resultStudent != null)
            {
                resultStudent.Name = s.Name;
                resultStudent.Age = s.Age;
                resultStudent.M1Marks = s.M1Marks;
                resultStudent.M2Marks = s.M2Marks;
                resultStudent.M3Marks = s.M3Marks;
                resultStudent.Courses = s.Courses;
                return "Student Updated Successfully";
            }
            else
            {
                return "Student does not exist with given Id";
            }
        }
    }
}
