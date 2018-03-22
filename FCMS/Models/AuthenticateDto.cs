using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FCMS.Models
{
    public class AuthenticateDto
    {
        public String EmployeeCardId { get; set; }

        public String Pin { get; set; }
    }

    public class AuthenticateDtoValidator : AbstractValidator<AuthenticateDto>
    {
        public AuthenticateDtoValidator()
        {
            this.RuleFor(x => x.EmployeeCardId).NotEmpty();
            this.RuleFor(x => x.Pin).NotNull().Length(4, 4);
        }
    }
}
