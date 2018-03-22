using FCMS.Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FCMS.Models
{
    public class UpdateEmployeeDto : IUpdateEmployeeModel
    {
        public String EmployeeId { get; set; }

        public String Name { get; set; }

        public String Email { get; set; }

        public String MobileNumber { get; set; }
    }


    public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
    {
        public UpdateEmployeeDtoValidator()
        {
            this.RuleFor(x => x.EmployeeId).NotEmpty();
            this.RuleFor(x => x.Name).NotEmpty();
            this.RuleFor(x => x.Email).NotEmpty().EmailAddress();
            this.RuleFor(x => x.MobileNumber).NotEmpty();
        }
    }
}
