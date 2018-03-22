using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FCMS.Models
{
    public class ChangePinDto
    {
        public String Pin { get; set; }

        public String NewPin { get; set; }
        
    }

    public class ChangePinDtoValidator : AbstractValidator<ChangePinDto>
    {
        public ChangePinDtoValidator()
        {
            this.RuleFor(x => x.Pin).NotNull().Length(4,4);
            this.RuleFor(x => x.NewPin).NotNull().Length(4, 4);
        }
    }
}
