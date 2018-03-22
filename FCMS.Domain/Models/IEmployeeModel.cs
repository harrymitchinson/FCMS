using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Domain.Models
{
    public interface IEmployeeModelBase { 
        String EmployeeId { get; set; }

        String Name { get; set; }

        String Email { get; set; }

        String MobileNumber { get; set; }
    }

    public interface IEmployeeModel : IEmployeeModelBase
    {
        String EmployeeCardId { get; set; }

        DateTime Created { get; set; }

        DateTime Modified { get; set; }
    }

    public interface ICreateEmployeeModel : IEmployeeModelBase
    {
        String EmployeeCardId { get; set; }
        String Pin { get; set; }
    }

    public interface IUpdateEmployeeModel : IEmployeeModelBase
    {

    }

}
