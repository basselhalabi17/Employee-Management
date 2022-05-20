using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext c;
        public SQLEmployeeRepository(AppDbContext con)
        {
            this.c = con;
        }
        public Employee add(Employee employee)
        {
            c.Employees.Add(employee);
            c.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee e =c.Employees.Find(id);
            if (e != null)
            {
                c.Employees.Remove(e);
                c.SaveChanges();
            }
            return e;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return c.Employees;
        }

        public Employee GetEmployee(int Id)
        {
            return c.Employees.Find(Id);
        }

        public Employee Update(Employee emp)
        {
           var x = c.Employees.Attach(emp);
            x.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            c.SaveChanges();
            return emp;
        }
    }
}
