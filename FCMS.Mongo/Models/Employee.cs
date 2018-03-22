using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Mongo.Models
{
    public class Employee
    {
        [BsonId, BsonRequired]
        public String EmployeeCardId { get; set; }
        [BsonRequired]
        public String EmployeeId { get; set; }
        [BsonRequired]
        public String Name { get; set; }
        [BsonRequired]
        public String Email { get; set; }
        [BsonRequired]
        public String MobileNumber { get; set; }
        [BsonRequired]
        public String Pin { get; set; }
        [BsonRequired]
        public DateTime Created { get; set; }
        [BsonRequired]
        public DateTime Modified { get; set; }
    }
}
