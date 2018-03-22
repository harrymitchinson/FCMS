using FCMS.Mongo.Models;
using FCMS.Mongo.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Mongo
{
    /// <summary>
    /// The employee data context.
    /// </summary>
    public class EmployeesContext
    {
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Initialises the database using options from DI. 
        /// Creates the database and collection if they do not already exist.
        /// </summary>
        /// <param name="options"></param>
        public EmployeesContext(IOptions<EmployeesContextOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            this.Database = client.GetDatabase(options.Value.DatabaseName);
            this.Employees = this.Database.GetCollection<Employee>(options.Value.CollectionName);            
        }

        /// <summary>
        /// The collection of employees in the database.
        /// </summary>
        public IMongoCollection<Employee> Employees { get; }
    }
}
