﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Fluent_Nhibernate.Entities;

namespace Fluent_Nhibernate.Mappings
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Price);
            HasManyToMany(x => x.StoresStockedIn)
              .Cascade.All()
              .Inverse()
              .Table("StoreProduct");
        }
    }
}
