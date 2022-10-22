using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    /*Repo pattern : Its an abstarction for DAL, it hides the details how exactly data is saved & retrived from 
     underlying database.The repository pattern atleast contain 2 partcipants 1)repoInterfaces 2)repo class implemnets Repointerface
    The repo interface only specifies the methods/operations supported by repository
    It doesn't contain the details how it will pweform that will be under immplemented rep class*/
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetAllEmployee();
        Employee Add(Employee employee);
        Employee Update(Employee employeeChanges);
        Employee Delete(int Id);
    }
}
