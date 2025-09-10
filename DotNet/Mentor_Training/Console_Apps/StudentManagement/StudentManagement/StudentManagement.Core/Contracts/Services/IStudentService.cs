using StudentManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Contracts.Services
{
    public interface IStudentService
    {
        public string AddStudent(Student s);
        public string DeleteStudent(int id);
        public string UpdateStudent(Student s);
        public Student GetStudent(int id);
        public List<Student> GetAllStudents();
        public List<Student> GetStudentsByAge(int age);
        public List<Student> GetStudnetsByCourseName(string courseName);
        public List<Course>? GetCoursesbyStudentId(int id);
        public string AddCoursesToStudent(int studentId, List<int> courseIds);
    }
}
