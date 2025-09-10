// See https://aka.ms/new-console-template for more information

using StudentManagement.Core.Contracts.Services;
using StudentManagement.Core.Vms;
using System;

namespace StudentManagement
{
    public class Program
    {
        private static readonly StudentVm studentVm = new StudentVm();
        private static readonly CourseVm courseVm = new CourseVm();

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Student Management System");
            int choice = 0;


            do
            {
                Console.WriteLine("Select the following services:");
                Console.WriteLine("1. Student Services");
                Console.WriteLine("2. Course Services");
                Console.WriteLine("3. Exit");

                choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        studentVm.StudentOptions();
                        break;

                    case 2:
                        courseVm.CourseOptions();
                        break;

                    case 3:
                        Console.WriteLine("Thank you for using Student Management System");
                        break;

                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                }
            } while (choice != 3);
        }
    }
}