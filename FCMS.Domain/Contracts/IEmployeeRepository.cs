using FCMS.Domain.Models;
using FCMS.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FCMS.Domain.Contracts
{
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Finds the employee by their employee card id. Returns the employee or null.
        /// </summary>
        /// <param name="employeeCardId"></param>
        /// <returns></returns>
        Task<IEmployeeModel> FindEmployeeByEmployeeCardIdAsync(String employeeCardId);

        /// <summary>
        /// Creates a new employee in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IEmployeeModel> CreateAsync(ICreateEmployeeModel model);

        /// <summary>
        /// Checks whether the supplied pin matches the employee's stored pin.
        /// </summary>
        /// <param name="employeeCardId"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        Task<Boolean> VerifyPinAsync(String employeeCardId, String pin);

        /// <summary>
        /// Updates the employee's details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Boolean> UpdateEmployeeAsync(String employeeCardId, IUpdateEmployeeModel model);

        /// <summary>
        /// Changes the employee's pin. Use VerifyPinAsync() before changing the pin to confirm the employee should be able to do this.
        /// </summary>
        /// <param name="employeeCardId"></param>
        /// <param name="newPin"></param>
        /// <returns></returns>
        Task<Boolean> ChangePinAsync(String employeeCardId, String newPin);
    }
}
