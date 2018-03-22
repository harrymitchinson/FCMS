using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Tests.FCMS.Controllers
{
    public class ControllerTestBase<T>
    {
        public ILogger<T> Logger;

        public ControllerTestBase()
        {
            var mockLogger = new Mock<ILogger<T>>();
            var logger = mockLogger.Object;
            this.Logger = logger;
        }
    }
}
