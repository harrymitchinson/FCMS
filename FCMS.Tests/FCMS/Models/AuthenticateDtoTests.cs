using FCMS.Models;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FCMS.Tests.FCMS.Models
{
    [Trait("Category", "Validators")]
    public class AuthenticateDtoTests
    {
        private AuthenticateDtoValidator Validator;
        public AuthenticateDtoTests()
        {
            this.Validator = new AuthenticateDtoValidator();
        }

        [Fact]
        public void Should_error_when_employeeCardId_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeCardId, null as String);
        }

        [Fact]
        public void Should_error_when_employeeCardId_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeCardId, String.Empty);
        }

        [Fact]
        public void Should_error_when_pin_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Pin, null as String);
        }

        [Fact]
        public void Should_error_when_pin_is_less_than_4_characters()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Pin, "123");
        }

        [Fact]
        public void Should_error_when_pin_is_more_than_4_characters()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Pin, "12345");
        }
    }
}
