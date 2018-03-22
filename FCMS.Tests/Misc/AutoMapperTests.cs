using AutoMapper;
using FCMS.Domain.Models;
using FCMS.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FCMS.Tests.Misc
{
    [Trait("Category", "AutoMapper")]
    public class AutoMapperTests
    {
        private readonly IMapper Mapper;
        public AutoMapperTests()
        {
            this.Mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        }

        [Fact]
        public void AutoMapper_should_be_configured_correctly()
        {
            // Act
            var exception = Record.Exception(() => this.Mapper.ConfigurationProvider.AssertConfigurationIsValid());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void EmployeeModel_should_not_throw_when_mapped_to_employee()
        {
            // Arrange
            var model = new EmployeeModel();

            // Act
            var ex = Record.Exception(() => this.Mapper.Map<Employee>(model));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void Employee_should_not_throw_when_mapped_to_employeeModel()
        {
            // Arrange
            var model = new Employee();

            // Act
            var ex = Record.Exception(() => this.Mapper.Map<EmployeeModel>(model));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void CreateEmployeeModel_should_not_throw_when_mapped_to_Employee()
        {
            // Arrange
            var model = new CreateEmployeeModel();

            // Act
            var ex = Record.Exception(() => this.Mapper.Map<Employee>(model));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void CreateEmployeeModel_should_set_created_property_when_mapped_to_Employee()
        {
            // Arrange
            var model = new CreateEmployeeModel();

            // Act
            var mapped = this.Mapper.Map<Employee>(model);

            // Assert
            Assert.NotEqual(DateTime.MinValue, mapped.Created);
        }

        [Fact]
        public void CreateEmployeeModel_should_set_modified_property_when_mapped_to_Employee()
        {
            // Arrange
            var model = new CreateEmployeeModel();

            // Act
            var mapped = this.Mapper.Map<Employee>(model);

            // Assert
            Assert.NotEqual(DateTime.MinValue, mapped.Modified);
        }
    }
}
