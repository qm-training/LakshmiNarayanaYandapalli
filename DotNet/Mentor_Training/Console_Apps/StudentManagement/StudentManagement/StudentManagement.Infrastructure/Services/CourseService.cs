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
    public class CourseService: ICourseService
    {
        public string AddCourse(Course c)
        {
            var resultCourse = GetCourse(c.Id);
            if (resultCourse == null)
            {
                CourseData.Courses.Add(c);
                return "Course Added Successfully";
            }
            else
            {
                return "Course with given Id already exists";
            }
        }

        public string DeleteCourse(int id)
        {
            var resultCourse = GetCourse(id);
            if (resultCourse != null)
            {
                foreach (var student in StudentData.Students)
                {
                    if (student.Courses != null)
                    {
                        foreach (var course in student.Courses)
                        {
                            if (course.Id == id)
                            {
                                return "Can not delete course one or more students registered with the course";
                            }
                        }
                    }
                }
                CourseData.Courses.Remove(resultCourse);
                return "Course Deleted Successfully";
            }
            else
            {
                return "Course with given Id does not exist";
            }
        }

        public List<Course> GetAllCourses()
        {
            return CourseData.Courses;
        }

        public Course GetCourse(int id)
        {
            foreach (Course c in CourseData.Courses)
            {
                if (c.Id == id)
                {
                    return c;
                }
            }
            return null;
        }

        public string UpdateCourse(Course c)
        {
            var resultCourse = GetCourse(c.Id);
            if (resultCourse != null)
            {
                resultCourse.Name = c.Name;
                resultCourse.Description = c.Description;
                return "Course Updated Successfully";
            }
            else
            {
                return "Course with given Id does not exist";
            }
        }
    }
}
