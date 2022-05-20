using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeelist;

        public MockEmployeeRepository()
        {
            _employeelist = new List<Employee>() {
                new Employee() { Id = 1, Name = "Mary", Department = Dept.HR, Email = "mary@pragimtech.com" },
                new Employee() { Id = 2, Name = "John", Department = Dept.IT, Email = "john@pragimtech.com" },
                new Employee() { Id = 3, Name = "Sam", Department = Dept.IT, Email = "sam@pragimtech.com" },
            };
        }

        public Employee add(Employee employee)
        {
          employee.Id =  _employeelist.Max(e => e.Id) + 1;
           _employeelist.Add(employee);
           return employee;
        }

        public Employee Delete(int id)
        {
            Employee empdel = _employeelist.FirstOrDefault(e => e.Id == id);
            if (empdel != null)
            {
                _employeelist.Remove(empdel);
            }
            return empdel;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeelist;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeelist.FirstOrDefault(emp => emp.Id == Id);
        }

        public Employee Update(Employee emp)
        {
            Employee empdel = _employeelist.FirstOrDefault(e => e.Id == emp.Id);
            if (empdel != null)
            {
                empdel.Name = emp.Name;
                empdel.Email = emp.Email;
                empdel.Department = emp.Department;
               
            }
            return empdel;
        }
    }
}
