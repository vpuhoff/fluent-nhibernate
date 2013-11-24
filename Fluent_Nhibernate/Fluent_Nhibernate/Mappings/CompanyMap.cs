using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Fluent_Nhibernate.Entities;

namespace Fluent_Nhibernate.Mappings
{
    public class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            HasMany(x => x.Project)
                .Inverse()
                .Cascade.All();
        }
    }
}
