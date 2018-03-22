using FCMS.Models;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FCMS.Tests.FCMS.Models
{
    [Trait("Category", "Validators")]
    public class ChangePinDtoTests
    {
        private ChangePinDtoValidator Validator;
        public ChangePinDtoTests()
        {
            this.Validator = new ChangePinDtoValidator();
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

        [Fact]
        public void Should_error_when_newPin_is_null()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.NewPin, null as String);
        }

        [Fact]
        public void Should_error_when_newPin_is_less_than_4_characters()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.NewPin, "123");
        }

        [Fact]
        public void Should_error_when_newPin_is_more_than_4_characters()
        {
            this.Validator.ShouldHaveValidationErrorFor(x => x.NewPin, "12345");
        }
    }
}
