using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FCMS.Domain.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FCMS.Controllers
{
    public class BaseController<T> : Controller
    {
        protected ILogger<T> Logger { get; }
        public BaseController(ILogger<T> logger)
        {
            this.Logger = logger;
        }
        private String _employeeCardId { get; set; }
        public String EmployeeCardId
        {
            get => this.User.HasClaim(x => x.Type == ClaimTypes.NameIdentifier) ? this.User.FindFirst(ClaimTypes.NameIdentifier).Value : this._employeeCardId;
            set => this._employeeCardId = value;
        }
    }
}