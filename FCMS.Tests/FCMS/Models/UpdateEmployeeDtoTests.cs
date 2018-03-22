using FCMS.Models;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FCMS.Tests.FCMS.Models
{
    [Trait("Category", "Validators")]
    public class UpdateEmployeeDtoTests
    {
        private UpdateEmployeeDtoValidator Validator;
        public UpdateEmployeeDtoTests()
        {
            this.Validator = new UpdateEmployeeDtoValidator();
        }

        [Fact]
        public void Should_error_when_employeeId_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeId, null as String);
        }

        [Fact]
        public void Should_error_when_employeeId_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeId, String.Empty);
        }

        [Fact]
        public void Should_error_when_name_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Name, null as String);
        }

        [Fact]
        public void Should_error_when_name_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Name, String.Empty);
        }

        [Fact]
        public void Should_error_when_email_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Email, null as String);
        }

        [Fact]
        public void Should_error_when_email_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Email, String.Empty);
        }

        [Fact]
        public void Should_error_when_mobileNumber_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.MobileNumber, null as String);
        }

        [Fact]
        public void Should_error_when_mobileNumber_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.MobileNumber, String.Empty);
        }
    }
}