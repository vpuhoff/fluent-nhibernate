using System;
using System.IO;
using Fluent_Nhibernate.Entities;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Linq;
using System.Collections.Generic;

namespace Examples.FirstProject
{
    class Program
    {
        private const string DbFile = "firstProgram.db";

        static void Main()
        {
            // create our NHibernate session factory
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                // populate the database
                using (var transaction = session.BeginTransaction())
                {
                    // create a couple of Stores each with some Products and Employees
                    var barginBasin = new Store { Name = "Bargin Basin" };
                    var superMart = new Store { Name = "SuperMart" };

                    var potatoes = new Product { Name = "Potatoes", Price = 3.60 };
                    var fish = new Product { Name = "Fish", Price = 4.49 };
                    var milk = new Product { Name = "Milk", Price = 0.79 };
                    var bread = new Product { Name = "Bread", Price = 1.29 };
                    var cheese = new Product { Name = "Cheese", Price = 2.10 };
                    var waffles = new Product { Name = "Waffles", Price = 2.41 };

                    var daisy = new Employee { FirstName = "Daisy", LastName = "Harrison" };
                    var jack = new Employee { FirstName = "Jack", LastName = "Torrance" };
                    var sue = new Employee { FirstName = "Sue", LastName = "Walkters" };
                    var bill = new Employee { FirstName = "Bill", LastName = "Taft" };
                    var joan = new Employee { FirstName = "Joan", LastName = "Pope" };

                    // create Projects and Companys
                    var project1 = new Project { Name = "Project 1", Client = "Client 1" };
                    var project2 = new Project { Name = "Project 2", Client = "Client 2" };
                    var project3 = new Project { Name = "Project 3", Client = "Client 3" };

                    var company1 = new Company { Name = "Company 1" };
                    var company2 = new Company { Name = "Company 2" };

                    // add products to the stores, there's some crossover in the products in each
                    // store, because the store-product relationship is many-to-many
                    AddProductsToStore(barginBasin, potatoes, fish, milk, bread, cheese);
                    AddProductsToStore(superMart, bread, cheese, waffles);

                    // add employees to the stores, this relationship is a one-to-many, so one
                    // employee can only work at one store at a time
                    AddEmployeesToStore(barginBasin, daisy, jack, sue);
                    AddEmployeesToStore(superMart, bill, joan);

                    // save both stores, this saves everything else via cascading
                    session.SaveOrUpdate(barginBasin);
                    session.SaveOrUpdate(superMart);

                    // add employees to the projects
                    AddEmployeesToProject(project1, daisy, jack, bill);
                    AddEmployeesToProject(project2, daisy, sue, joan, jack);

                    //add projects to the company
                    AddProjectsToCompany(company1, project1, project3);
                    AddProjectsToCompany(company2, project2);

                    // save projects, this saves everything else via cascading
                    session.SaveOrUpdate(company1);
                    session.SaveOrUpdate(company2);

                    transaction.Commit();
                }
            }

            using (var session = sessionFactory.OpenSession())
            {

                using (session.BeginTransaction())
                {
                    #region retrieve all products which have Store name like HVN
                    
                    var productsLinQ = (from p in session.Query<Product>()
                                        where p.StoresStockedIn.Any(x => x.Name.Contains("HVN"))
                                        orderby p.Name
                                        select p).ToList<Product>();
                    Console.WriteLine("Retrieve all products which have Store name like HVN. Write by LinQ");
                    foreach (Product pro in productsLinQ)
                    {
                        Console.WriteLine(pro.Name);
                    }
                    Console.WriteLine();

                    var productsNativeSQL = session.CreateSQLQuery("select pro.Name, pro.Id, pro.Price from Product pro, StoreProduct sp, Store sto where pro.Id = sp.Product_id and sp.Store_id = sto.Id and sto.Name like '%HVN%' order by pro.Name")
                            .AddEntity(typeof(Product)).List();

                    Console.WriteLine("Retrieve all products which have Store name like HVN. Write by Native SQL");
                    foreach (Product pro in productsNativeSQL)
                    {
                        Console.WriteLine(pro.Name);
                    }
                    Console.WriteLine();
                    #endregion

                    #region retrieve all Employee which have Produce price > 3000
                    
                    var employeeLinQ = (from p in session.Query<Employee>()
                                        where p.Store.Products.Any(x => x.Price > 3000)
                                        orderby p.FirstName
                                        select p).ToList<Employee>();
                    Console.WriteLine("Retrieve all Employee which have Produce price > 3000. Write by LinQ");
                    foreach (Employee empoyee in employeeLinQ)
                    {
                        Console.WriteLine(empoyee.FirstName + " " + empoyee.LastName);
                    }
                    Console.WriteLine();

                    var employeeNativeSQL = session.CreateSQLQuery(" select emp.FirstName, emp.Id, emp.LastName from Employee emp, StoreProduct sp, Store sto, Product pro where emp.Store_id = sto.Id and pro.Id = sp.Product_id and sp.Store_id = sto.Id and pro.Price > 3000")
                            .AddEntity(typeof(Employee)).List();

                    Console.WriteLine("Retrieve all Employee which have Produce price > 3000. Write by NativeSQL");
                    foreach (Employee pro in employeeNativeSQL)
                    {
                        Console.WriteLine(pro.FirstName);
                    }
                    Console.WriteLine();
            #endregion
                
                    // retrieve all Projects and their employees
                    //var project = session.Query<Project>().ToList<Project>();
                    //foreach (Project pro in project)
                    //{
                    //    Console.WriteLine(pro.Name);
                    //    pro.Employee.ForEach(x => Console.Write(x.FirstName + "    "));
                    //    Console.WriteLine();
                    //}
                    
                    //// retrieve all Company and their Projects
                    //var companys = session.Query<Company>().ToList<Company>();
                    //foreach (Company company in companys)
                    //{
                    //    Console.WriteLine(company.Name);
                    //    company.Project.ForEach(x => Console.Write(x.Name + "    "));
                    //    Console.WriteLine();
                    //}
                }
            }

            Console.ReadKey();
        }

        private static void AddProjectsToCompany(Company company, params Project[] projects)
        {
            foreach (Project project in projects)
            {
                company.AddProject(project);
            }
        }

        private static void AddEmployeesToProject(Project project, params Employee[] employees)
        {
            foreach (Employee employee in employees)
            {
                project.AddEmployee(employee);
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard
                    .UsingFile(DbFile))
                .Mappings(m =>
                    m.FluentMappings.AddFromAssemblyOf<Program>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(DbFile))
                File.Delete(DbFile);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .Create(false, true);
        }

        private static void WriteStorePretty(Store store)
        {
            Console.WriteLine(store.Name);
            Console.WriteLine(" Products:");

            foreach (var product in store.Products)
            {
                Console.WriteLine(" " + product.Name);
            }

            Console.WriteLine(" Staff:");

            foreach (var employee in store.Staff)
            {
                Console.WriteLine(" " + employee.FirstName + " " + employee.LastName);
            }

            Console.WriteLine();
        }

        public static void AddProductsToStore(Store store, params Product[] products)
        {
            foreach (var product in products)
            {
                store.AddProduct(product);
            }
        }

        public static void AddEmployeesToStore(Store store, params Employee[] employees)
        {
            foreach (var employee in employees)
            {
                store.AddEmployee(employee);
            }
        }
    }
}