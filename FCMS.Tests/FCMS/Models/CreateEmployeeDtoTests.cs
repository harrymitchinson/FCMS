using FCMS.Models;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FCMS.Tests.FCMS.Models
{
[Trait("Category", "Validators")]
public class CreateEmployeeDtoTests
{
    private CreateEmployeeDtoValidator Validator;
    public CreateEmployeeDtoTests()
    {
        this.Validator = new CreateEmployeeDtoValidator();
    }

    [Fact]
    public void Should_error_when_EmployeeCardId_is_null()
    {
        this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeCardId, null as String);
    }

    [Fact]
    public void Should_error_when_EmployeeCardId_is_empty()
    {
        this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeCardId, String.Empty);
    }

    // Continues...

        [Fact]
        public void Should_error_when_EmployeeId_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeId, null as String);
        }

        [Fact]
        public void Should_error_when_EmployeeId_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.EmployeeId, String.Empty);
        }

        [Fact]
        public void Should_error_when_Name_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Name, null as String);
        }

        [Fact]
        public void Should_error_when_Name_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Name, String.Empty);
        }

        [Fact]
        public void Should_error_when_Email_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Email, null as String);
        }

        [Fact]
        public void Should_error_when_Email_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Email, String.Empty);
        }

        [Fact]
        public void Should_error_when_MobileNumber_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.MobileNumber, null as String);
        }

        [Fact]
        public void Should_error_when_MobileNumber_is_empty()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.MobileNumber, String.Empty);
        }

        [Fact]
        public void Should_error_when_Pin_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Pin, null as String);
        }

        [Fact]
        public void Should_error_when_Pin_is_less_than_4_characters()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Pin, "123");
        }

        [Fact]
        public void Should_error_when_Pin_is_more_than_4_characters()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.Pin, "12345");
        }
    }
}
