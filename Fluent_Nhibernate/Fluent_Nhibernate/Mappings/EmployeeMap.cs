﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Fluent_Nhibernate.Entities;

namespace Fluent_Nhibernate.Mappings
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            References(x => x.Store);
            HasManyToMany(x => x.Project)
             .Cascade.All()
             .Table("ProjectEmploye");
        }
    }
}
