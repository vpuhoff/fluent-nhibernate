using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluent_Nhibernate.Entities;
namespace Fluent_Nhibernate.Entities
{
    public class Project
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Client { get; set; }
        public virtual Company Company { get; set; }
        public virtual IList<Employee> Employee { get; set; }

        public Project()
        {
            Employee = new List<Employee>();
        }
        public virtual void AddEmployee(Employee employee)
        {
            employee.Project.Add(this);
            Employee.Add(employee);
        }
    }
}
