using FCMS.Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FCMS.Models
{
    public class CreateEmployeeDto : ICreateEmployeeModel
    {
        public String EmployeeCardId { get; set; }

        public String EmployeeId { get; set; }

        public String Name { get; set; }

        public String Email { get; set; }

        public String MobileNumber { get; set; }

        public String Pin { get; set; }
    }

    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            this.RuleFor(x => x.EmployeeCardId).NotEmpty();
            this.RuleFor(x => x.Name).NotEmpty();
            this.RuleFor(x => x.Email).NotEmpty().EmailAddress();
            this.RuleFor(x => x.EmployeeId).NotEmpty();
            this.RuleFor(x => x.MobileNumber).NotEmpty();
            this.RuleFor(x => x.Pin).NotNull().Length(4, 4);
        }
    }
}
