using StudentManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Contracts.Services
{
    public interface ICourseService
    {
        public String AddCourse(Course c);
        public String DeleteCourse(int id);
        public String UpdateCourse(Course c);
        public Course GetCourse(int id);
        public List<Course> GetAllCourses();
    }
}
