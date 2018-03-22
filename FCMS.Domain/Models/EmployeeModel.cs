using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FCMS.Domain.Models
{
    public abstract class EmployeeModelBase : IEmployeeModelBase
    {
        public String EmployeeId { get; set; }

        public String Name { get; set; }

        public String Email { get; set; }

        public String MobileNumber { get; set; }
    }

    public class EmployeeModel : EmployeeModelBase, IEmployeeModel
    {
        public String EmployeeCardId { get; set; }
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }

    public class CreateEmployeeModel : EmployeeModelBase, ICreateEmployeeModel
    {
        public String EmployeeCardId { get; set; }
        public String Pin { get; set; }
    }

    public class UpdateEmployeeModel : EmployeeModelBase, IUpdateEmployeeModel
    {

    }
}

