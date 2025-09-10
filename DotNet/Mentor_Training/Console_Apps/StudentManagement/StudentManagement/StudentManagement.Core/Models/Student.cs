using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }

        public double M1Marks { get; set; }
        public double M2Marks { get; set; }
        public double M3Marks { get; set; }
        public static int NumOfStudents { get; private set; } = 0;
        public List<Course> Courses { get; set; }
    }
}
