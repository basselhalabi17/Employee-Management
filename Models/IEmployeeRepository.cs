using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id) ;
        IEnumerable<Employee> GetAllEmployee();
        Employee add(Employee employee);
        Employee Update(Employee emp);
        Employee Delete(int id);
    }
}
