using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    /*The migrations keeps the dbscheme and application model classes in sync
     To work with migrations ww will either use package manager console/.netCLI(cross platform)
    add-migration >> migration.cs file in migrations folder
    update-database >> for executing migrations, we have to specify migrations if not latest migration will be applied
    we can look at package manager console logs- what is created

    */
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;

        //here want to work with sqlentities so we are injecting appdbcontext
        public SQLEmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Employee Add(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public Employee Delete(int Id)
        {
            Employee employee = context.Employees.Find(Id);
            if (employee != null)
            {
                context.Employees.Remove(employee);
                context.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return context.Employees;
        }

        public Employee GetEmployee(int Id)
        {
            return context.Employees.Find(Id);
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = context.Employees.Attach(employeeChanges);
            /*From the intellisense of Attach it will return entry entity,
            we have to tell EF that the entity we have attached is modified*/
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employeeChanges;
        }
    }
}
