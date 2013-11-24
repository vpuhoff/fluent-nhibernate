using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent_Nhibernate.Entities
{
    public class Company
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<Project> Project { get; set; }

        public Company()
        {
            Project = new List<Project>();
        }
        public virtual void AddProject(Project project)
        {
            project.Company = this;
            Project.Add(project);
        }
    }
}
