using System;
using System.Threading.Tasks;
using FCMS.Domain.Contracts;
using FCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FCMS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmployeesController : BaseController<EmployeesController>
    {
        private IEmployeeRepository EmployeeRepository { get; }

        private IJwtRepository JwtRepository { get; }

        public EmployeesController(IEmployeeRepository employeeRepository, IJwtRepository jwtRepository, ILogger<EmployeesController> logger) : base(logger)
        {
            this.EmployeeRepository = employeeRepository;
            this.JwtRepository = jwtRepository;
        }

        /// <summary>
        /// GET api/employees/{employeeCardId}
        /// Finds the employee who has the provided employeeCardId and returns it otherwise returns a 404 if the employee is not found.
        /// </summary>
        /// <param name="employeeCardId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{employeeCardId}")]
        public async Task<IActionResult> GetEmployeeAsync([FromRoute] String employeeCardId)
        {
            try
            {
                // Find the employee.
                var employee = await this.EmployeeRepository.FindEmployeeByEmployeeCardIdAsync(employeeCardId);

                // If the employee is null return 404 since the employee was not found.
                if (employee == null)
                {
                    return this.NotFound();
                }

                // Return the employee.
                return this.Ok(employee);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error when GetEmployeeAsync for {0}", employeeCardId);
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// POST api/employees
        /// Creates a new employee. Returns a JWT for this employee to use in future requests.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] CreateEmployeeDto model)
        {
            try
            {
                // Create the employee.
                var employee = await this.EmployeeRepository.CreateAsync(model);

                // Create a JWT for this employee.
                var token = this.JwtRepository.Create(employee);

                // Return the JWT.
                return this.Ok(token);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error when CreateEmployeeAsync for employeeCardId {0}, employeeId {1}, email {2}, name {3}",
                    model.EmployeeCardId,
                    model.EmployeeId,
                    model.Email,
                    model.Name);
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// POST api/employees/authenticate
        /// Authenticates an employee when provided with an employeeCardId and the correct pin for that employee, producing a JWT which can be used in future requests.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateDto model)
        {
            try
            {
                // Verify that the pin is correct for the provided employeeCardId.
                var authenticated = await this.EmployeeRepository.VerifyPinAsync(model.EmployeeCardId, model.Pin);
                if (!authenticated)
                {
                    return this.Unauthorized();
                }

                // Fetch the employee from the database.
                var employee = await this.EmployeeRepository.FindEmployeeByEmployeeCardIdAsync(model.EmployeeCardId);

                // Create a JWT for this employee.
                var token = this.JwtRepository.Create(employee);

                // Return the JWT.
                return this.Ok(token);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error when AuthenticateAsync for employeeCardId {0}", model.EmployeeCardId);
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// PUT api/employees
        /// Updates an employee in the database. Returns true or false depending on if an employee was successfully updated.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateEmployeeAsync([FromBody] UpdateEmployeeDto model)
        {
            try
            {
                // Update the employee using the employeeCardId from the auth token.
                var updated = await this.EmployeeRepository.UpdateEmployeeAsync(this.EmployeeCardId, model);

                // Return the result of the update.
                return this.Ok(updated);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error when UpdateEmployeeAsync for employeeCardId {0}", this.EmployeeCardId);
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// PUT api/employees/pin
        /// Updates an employee's pin in the database. Returns true or false depending on if an employee pin was successfully updated.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("pin")]
        [Authorize]
        public async Task<IActionResult> ChangePinAsync([FromBody] ChangePinDto model)
        {
            try
            {
                // Confirm that the provided emplyoee pin is correct.
                var authenticationConfirmed = await this.EmployeeRepository.VerifyPinAsync(this.EmployeeCardId, model.Pin);
                if (!authenticationConfirmed)
                {
                    return this.Unauthorized();
                }

                // Update the pin using the new pin.
                var updated = await this.EmployeeRepository.ChangePinAsync(this.EmployeeCardId, model.NewPin);

                // Return the result of the update.
                return this.Ok(updated);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error when ChangePinAsync for employeeCardId {0}", this.EmployeeCardId);
                return this.StatusCode(500);
            }
        }
    }
}