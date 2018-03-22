using FCMS.Mongo;
using FCMS.Mongo.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.Json;
using Xunit;
using FCMS.Domain.Contracts;
using FCMS.Domain.Repositories;
using System.Threading.Tasks;
using FCMS.Mongo.Models;
using MongoDB.Driver;
using AutoMapper;
using FCMS;
using Microsoft.AspNetCore.Identity;
using FCMS.Domain.Models;
using FCMS.Models;

namespace FCMS.Tests.Domain.Repositories
{
    public class EmployeeRepositoryFixture : IDisposable
    {
        public EmployeesContext Context;
        public String CollectionName;
        public IMapper Mapper;
        public IPasswordHasher<Employee> PasswordHasher;

        public EmployeeRepositoryFixture()
        {
            // Collect the configuration settings.
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            var options = Options.Create(configuration.GetSection("EmployeesContextTest").Get<EmployeesContextOptions>());

            // Create the context and repository.
            this.Context = new EmployeesContext(options);
            this.CollectionName = options.Value.CollectionName;

            // Register the AutoMapper profile.
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
            this.Mapper = mapper;

            // Register the password hasher.
            this.PasswordHasher = new PasswordHasher<Employee>();

            // Clear the test db.
            this.Context.Database.DropCollection(this.CollectionName);
        }

        public void Dispose()
        {

        }
    }

    [Trait("Category", "Services")]
    public class EmployeeRepositoryTests: RepositoryTestBase<EmployeeRepository>, IClassFixture<EmployeeRepositoryFixture>
    {
        private IEmployeeRepository EmployeeRepository { get; }
        private EmployeesContext Context { get; }
        private IPasswordHasher<Employee> PasswordHasher { get; }
        public EmployeeRepositoryTests(EmployeeRepositoryFixture fixture) : base()
        {
            this.EmployeeRepository = new EmployeeRepository(fixture.Context, fixture.PasswordHasher, fixture.Mapper, this.Logger);
            this.Context = fixture.Context;
            this.PasswordHasher = fixture.PasswordHasher;
        }

        [Fact]
        public async Task FindEmployeeByEmployeeCardIdAsync_should_return_null_when_not_found()
        {
            // Arange
            var id = Guid.NewGuid().ToString();

            // Act
            var result = await this.EmployeeRepository.FindEmployeeByEmployeeCardIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindEmployeeByEmployeeCardIdAsync_should_return_employee_when_found()
        {
            // Arange
            var employee = new Employee
            {
                Email = "test@test.com",
                EmployeeCardId = Guid.NewGuid().ToString(),
                EmployeeId = Guid.NewGuid().ToString(),
                MobileNumber = "07777777777",
                Name = "Joe Bloggs",
                Pin = "0123"            
            };
            await this.Context.Employees.InsertOneAsync(employee);

            // Act
            var result = await this.EmployeeRepository.FindEmployeeByEmployeeCardIdAsync(employee.EmployeeCardId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateAsync_should_create_new_employee()
        {
            // Arange
            var employee = new CreateEmployeeDto
            {
                Email = "test@test.com",
                EmployeeCardId = Guid.NewGuid().ToString(),
                EmployeeId = Guid.NewGuid().ToString(),
                MobileNumber = "07777777777",
                Name = "Joe Bloggs",
                Pin = "0123"
            };

            // Act
            await this.EmployeeRepository.CreateAsync(employee);

            // Assert 
            var filter = Builders<Employee>.Filter.Eq(x => x.EmployeeCardId, employee.EmployeeCardId);
            var result = await this.Context.Employees.Find(filter).FirstOrDefaultAsync();
            Assert.NotEqual(DateTime.MinValue, result.Created);
        }

        [Fact]
        public async Task CreateAsync_should_hash_pin()
        {
            // Arange
            var employee = new CreateEmployeeModel
            {
                Email = "test@test.com",
                EmployeeCardId = Guid.NewGuid().ToString(),
                EmployeeId = Guid.NewGuid().ToString(),
                MobileNumber = "07777777777",
                Name = "Joe Bloggs",
                Pin = "0123"
            };
            var originalPin = employee.Pin;

            // Act
            await this.EmployeeRepository.CreateAsync(employee);

            // Assert 
            var filter = Builders<Employee>.Filter.Eq(x => x.EmployeeCardId, employee.EmployeeCardId);
            var result = await this.Context.Employees.Find(filter).FirstOrDefaultAsync();

            Assert.NotEqual(originalPin, result.Pin);
            var verify = this.PasswordHasher.VerifyHashedPassword(result, result.Pin, originalPin);
            Assert.Equal(PasswordVerificationResult.Success, verify);
        }

        [Fact]
        public async Task VerifyPinAsync_should_return_true_when_verified()
        {
            // Arange
            var pin = "0123";
            var employee = new CreateEmployeeModel
            {
                Email = "test@test.com",
                EmployeeCardId = Guid.NewGuid().ToString(),
                EmployeeId = Guid.NewGuid().ToString(),
                MobileNumber = "07777777777",
                Name = "Joe Bloggs",
                Pin = pin
            };
            await this.EmployeeRepository.CreateAsync(employee);

            // Act
            var result = await this.EmployeeRepository.VerifyPinAsync(employee.EmployeeCardId, pin);

            // Assert 
            Assert.True(result);
        }

        [Fact]
        public async Task VerifyPinAsync_should_return_false_when_not_verified()
        {
            // Arange
            var employee = new CreateEmployeeModel
            {
                Email = "test@test.com",
                EmployeeCardId = Guid.NewGuid().ToString(),
                EmployeeId = Guid.NewGuid().ToString(),
                MobileNumber = "07777777777",
                Name = "Joe Bloggs",
                Pin = "0123"
            };
            await this.EmployeeRepository.CreateAsync(employee);

            // Act
            var result = await this.EmployeeRepository.VerifyPinAsync(employee.EmployeeCardId, "9999");

            // Assert 
            Assert.False(result);
        }

        [Fact]
        public async Task VerifyPinAsync_should_return_false_when_not_found()
        {
            // Arange
            var pin = "0123";

            // Act
            var result = await this.EmployeeRepository.VerifyPinAsync(Guid.NewGuid().ToString(), pin);

            // Assert 
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_should_return_true_when_updated()
        {
            // Arange
            var employee = new Employee
            {
                Email = "test@test.com",
                EmployeeCardId = Guid.NewGuid().ToString(),
                EmployeeId = Guid.NewGuid().ToString(),
                MobileNumber = "07777777777",
                Name = "Joe Bloggs",
                Pin = "0123"
            };
            await this.Context.Employees.InsertOneAsync(employee);

            var updateModel = new UpdateEmployeeModel
            {
                Email = "updated",
                EmployeeId = "updated",
                MobileNumber = "updated",
                Name = "updated"
            };

            // Act
            var result = await this.EmployeeRepository.UpdateEmployeeAsync(employee.EmployeeCardId, updateModel);

            // Assert 
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_should_return_false_when_not_updated()
        {
            // Arrange
            var updateModel = new UpdateEmployeeModel
            {
                Email = "updated",
                EmployeeId = "updated",
                MobileNumber = "updated",
                Name = "updated"
            };

            // Act
            var result = await this.EmployeeRepository.UpdateEmployeeAsync(Guid.NewGuid().ToString(), updateModel);

            // Assert 
            Assert.False(result);
        }

        [Fact]
        public async Task ChangePinAsync_should_return_true_when_changed()
        {
            // Arange
            var pin = "0123";
            var employee = new Employee
            {
                EmployeeCardId = Guid.NewGuid().ToString(),
                Pin = pin
            };
            await this.Context.Employees.InsertOneAsync(employee);

            // Act
            var result = await this.EmployeeRepository.ChangePinAsync(employee.EmployeeCardId, "9999");

            // Assert 
            Assert.True(result);
        }

        [Fact]
        public async Task ChangePinAsync_should_return_false_when_not_changed()
        {
            // Arange
            var pin = "0123";

            // Act
            var result = await this.EmployeeRepository.ChangePinAsync(Guid.NewGuid().ToString(), pin);

            // Assert 
            Assert.False(result);
        }
    }
}
