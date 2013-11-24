using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Fluent_Nhibernate.Entities;

namespace Fluent_Nhibernate.Mappings
{
    public class ProjectMap : ClassMap<Project>
    {
        public ProjectMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Client);
            References(x => x.Company)
                .Cascade.All();
            HasManyToMany(x => x.Employee)
                .Cascade.All()
                .Inverse()
                .Table("ProjectEmploye");
        }
    }
}
