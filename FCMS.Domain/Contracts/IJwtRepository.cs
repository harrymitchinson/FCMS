using FCMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Domain.Contracts
{
    public interface IJwtRepository
    {
        /// <summary>
        /// Creates a JSON Web Token (JWT) from a provided employee.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        String Create(IEmployeeModel employee);

        /// <summary>
        /// Checks whether the supplied value is a JWT or not.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean IsToken(String token);

        /// <summary>
        /// Validates a provided JWT. If the employee has a valid JWT they should be authenticated.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Validate(String token);
    }
}
