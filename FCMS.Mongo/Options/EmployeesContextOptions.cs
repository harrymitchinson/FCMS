using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Mongo.Options
{
    public class EmployeesContextOptions
    {
        public String ConnectionString { get; set; }
        public String DatabaseName { get; set; }
        public String CollectionName { get; set; }
    }
}
