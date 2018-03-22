using FCMS.Domain.Contracts;
using FCMS.Mongo;
using FCMS.Mongo.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using FCMS.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace FCMS.Domain.Repositories
{
    public class EmployeeRepository : RepositoryBase<EmployeeRepository>, IEmployeeRepository
    {
        private EmployeesContext Context { get; }
        private IMapper Mapper { get; }
        private IPasswordHasher<Employee> PasswordHasher { get; }
        public EmployeeRepository(EmployeesContext ctx, IPasswordHasher<Employee> ph, IMapper m, ILogger<EmployeeRepository> log) : base(log)
        {
            this.Context = ctx;
            this.PasswordHasher = ph;
            this.Mapper = m;
        }

        public async Task<IEmployeeModel> FindEmployeeByEmployeeCardIdAsync(String employeeCardId)
        {
            try
            {
                // Create a filter for matching the employeeCardId
                var filter = Builders<Employee>.Filter.Eq(x => x.EmployeeCardId, employeeCardId);

                // Execute a query with the filter.
                var employee = await this.Context.Employees.Find(filter).FirstOrDefaultAsync();

                // Map the Employee to an IEmployeeModel
                var result = this.Mapper.Map<IEmployeeModel>(employee);

                // Return the EmployeeModel
                return result;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error finding employee for employeeCardId {0}", employeeCardId);
                throw;
            }
        }

        // ...

        public async Task<IEmployeeModel> CreateAsync(ICreateEmployeeModel employee)
        {
            try
            {
                // Map the CreateEmployeeModel to an Employee which will populate the date properties.
                var mapped = this.Mapper.Map<Employee>(employee);

                // Hash the password.
                mapped.Pin = this.PasswordHasher.HashPassword(mapped, employee.Pin);

                // Add the employee to the database.
                await this.Context.Employees.InsertOneAsync(mapped);

                // Map the Employee to an EmployeeModel
                var result = this.Mapper.Map<IEmployeeModel>(mapped);

                // Return the EmployeeModel
                return result;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error creating employee card id {0}, employee id {1}, email {2}",
                    employee.EmployeeCardId,
                    employee.EmployeeId,
                    employee.Email);
                throw;
            }
        }

        public async Task<Boolean> VerifyPinAsync(String employeeCardId, String pin)
        {
            try
            {
                // Create a filter for matching the employeeCardId
                var filter = Builders<Employee>.Filter.Eq(x => x.EmployeeCardId, employeeCardId);

                // Execute a query with the filter.
                var employee = await this.Context.Employees.Find(filter).FirstOrDefaultAsync();
                if (employee == null)
                {
                    return false;
                }

                var verifyResult = this.PasswordHasher.VerifyHashedPassword(employee, employee.Pin, pin);
                if (verifyResult == PasswordVerificationResult.Success || verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    // TODO: Rehash password if it ever comes back as SuccessRehashNeeded.
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error verifying pin for employee card id {0}", employeeCardId);
                throw;
            }
        }

        public async Task<Boolean> UpdateEmployeeAsync(String employeeCardId, IUpdateEmployeeModel model)
        {
            try
            {
                // Create a filter for matching the employeeCardId
                var filter = Builders<Employee>.Filter.Eq(x => x.EmployeeCardId, employeeCardId);

                // Create the update definition.
                var update = Builders<Employee>.Update
                    .Set(x => x.EmployeeId, model.EmployeeId)
                    .Set(x => x.MobileNumber, model.MobileNumber)
                    .Set(x => x.Name, model.Name)
                    .Set(x => x.Email, model.Email)
                    .Set(x => x.Modified, DateTime.UtcNow);
                    
                // Execute a update definition query with the filter.
                var updated = await this.Context.Employees.UpdateOneAsync(filter, update);
                return updated.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error updating employee details for employee card id {0} with employee id {1}, mobile {2}, name {3}, email {4}",
                    employeeCardId,
                    model.EmployeeId,
                    model.MobileNumber,
                    model.Name,
                    model.Email);
                throw;
            }
        }

        public async Task<Boolean> ChangePinAsync(String employeeCardId, String newPin)
        {
            try
            {
                // Create a filter for matching the employeeCardId
                var filter = Builders<Employee>.Filter.Eq(x => x.EmployeeCardId, employeeCardId);

                // Execute a query with the filter and check there is a matching employee.
                var employee = await this.Context.Employees.Find(filter).FirstOrDefaultAsync();
                if (employee == null)
                {
                    return false;
                }

                // We should have already checked the employee knows the correct pin using VerifyPinAsync.

                // Hash the new pin.
                var hashedNewPin = this.PasswordHasher.HashPassword(employee, newPin);

                // Create the update definition.
                var update = Builders<Employee>.Update.Set(x => x.Pin, hashedNewPin).Set(x => x.Modified, DateTime.UtcNow);

                // Execute the update.
                var updated = await this.Context.Employees.UpdateOneAsync(filter, update);

                // Return true if an entry was updated.
                return updated.ModifiedCount == 1;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error changing employee pin for employee card id {0}",
                    employeeCardId);
                throw;
            }
        }
    }
}
