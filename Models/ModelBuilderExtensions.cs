using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtensions
    {
        //the seed method is extension class
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //HasData() will accept array
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "mark",
                    Department = Dept.IT,
                    Email = "mark@gmail.com"
                },
                 new Employee
                 {
                     Id = 2,
                     Name = "mark1",
                     Department = Dept.IT,
                     Email = "mark1@gmail.com"
                 }
                );
            
        }
    }
}
