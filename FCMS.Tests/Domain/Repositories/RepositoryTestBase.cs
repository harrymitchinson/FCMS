using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Tests.Domain.Repositories
{
    public class RepositoryTestBase<T>
    {
        public ILogger<T> Logger;

        public RepositoryTestBase()
        {
            var mockLogger = new Mock<ILogger<T>>();
            var logger = mockLogger.Object;
            this.Logger = logger;
        }
    }
}
