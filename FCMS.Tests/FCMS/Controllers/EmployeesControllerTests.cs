using FCMS.Controllers;
using FCMS.Domain.Contracts;
using FCMS.Domain.Models;
using FCMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FCMS.Tests.FCMS.Controllers
{
    public class EmployeesControllerFixture : IDisposable
    {
        public IJwtRepository JwtRepository;
        public IEmployeeRepository EmployeeRepository;
        public String ValidId = Guid.NewGuid().ToString();
        public String InvalidId = Guid.NewGuid().ToString();

        public EmployeesControllerFixture()
        {
            var mockJwtRepo = new Mock<IJwtRepository>();
            mockJwtRepo.Setup(x => x.Create(It.IsAny<IEmployeeModel>())).Returns(() => "JWT");
            this.JwtRepository = mockJwtRepo.Object;

            var mockEmployeeRepo = new Mock<IEmployeeRepository>();
            mockEmployeeRepo.Setup(x => x.FindEmployeeByEmployeeCardIdAsync(It.Is<String>(y => y == this.ValidId))).Returns<String>(async x => await Task.FromResult(new EmployeeModel()));
            mockEmployeeRepo.Setup(x => x.FindEmployeeByEmployeeCardIdAsync(It.Is<String>(y => y == this.InvalidId))).Returns(async () => await Task.FromResult<EmployeeModel>(null));
            mockEmployeeRepo.Setup(x => x.CreateAsync(It.Is<ICreateEmployeeModel>(y => y.EmployeeCardId == this.ValidId))).Returns<ICreateEmployeeModel>(async x => await Task.FromResult(new EmployeeModel()));
            // TODO: Mock throw when Id already exists.
            mockEmployeeRepo.Setup(x => x.VerifyPinAsync(It.Is<String>(y => y == this.ValidId), It.IsAny<String>())).Returns(async () => await Task.FromResult(true));
            mockEmployeeRepo.Setup(x => x.VerifyPinAsync(It.Is<String>(y => y == this.InvalidId), It.IsAny<String>())).Returns(async () => await Task.FromResult(false));
            mockEmployeeRepo.Setup(x => x.UpdateEmployeeAsync(It.Is<String>(y => y == this.ValidId), It.IsAny<IUpdateEmployeeModel>())).Returns(async () => await Task.FromResult(true));
            mockEmployeeRepo.Setup(x => x.UpdateEmployeeAsync(It.Is<String>(y => y == this.InvalidId), It.IsAny<IUpdateEmployeeModel>())).Returns(async () => await Task.FromResult(false));
            mockEmployeeRepo.Setup(x => x.ChangePinAsync(It.Is<String>(y => y == this.ValidId), It.IsAny<String>())).Returns(async () => await Task.FromResult(true));
            mockEmployeeRepo.Setup(x => x.ChangePinAsync(It.Is<String>(y => y == this.InvalidId), It.IsAny<String>())).Returns(async () => await Task.FromResult(false));
            this.EmployeeRepository = mockEmployeeRepo.Object;
        }

        public void Dispose()
        {

        }
    }

    [Trait("Category", "Controllers")]
    public class EmployeesControllerTests : ControllerTestBase<EmployeesController>, IClassFixture<EmployeesControllerFixture>
    {
        public EmployeesControllerFixture Fixture { get; }

        public EmployeesControllerTests(EmployeesControllerFixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact]
        public async Task GetEmployeeAsync_should_return_404_when_employee_not_found()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger);

            // Act
            var response = await controller.GetEmployeeAsync(this.Fixture.InvalidId);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetEmployeeAsync_should_return_200_with_employee_when_employee_found()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger);

            // Act
            var response = await controller.GetEmployeeAsync(this.Fixture.ValidId);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var model = result.Value as EmployeeModel;
            Assert.NotNull(model);
        }

        [Fact]
        public async Task GetEmployeeAsync_should_return_500_when_error()
        {
            // Arrange
            var controller = new EmployeesController(null, null, this.Logger);

            // Act
            var response = await controller.GetEmployeeAsync(this.Fixture.ValidId);

            // Assert
            var result = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task CreateEmployeeAsync_should_return_200_with_employee_when_created()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger);
            var model = new CreateEmployeeDto { EmployeeCardId = this.Fixture.ValidId };

            // Act
            var response = await controller.CreateEmployeeAsync(model);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var resultModel = result.Value as EmployeeModel;
            Assert.NotNull(model);
        }

        [Fact]
        public async Task CreateEmployeeAsync_should_return_500_when_error()
        {
            // Arrange
            var controller = new EmployeesController(null, null, this.Logger);

            // Act
            var response = await controller.GetEmployeeAsync(this.Fixture.ValidId);

            // Assert
            var result = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task AuthenticateAsync_should_return_200_with_jwt_when_authenticated()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger);
            var model = new AuthenticateDto { EmployeeCardId = this.Fixture.ValidId, Pin = "1234" };

            // Act
            var response = await controller.AuthenticateAsync(model);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var jwt = result.Value as String;
            Assert.False(String.IsNullOrEmpty(jwt));
        }

        [Fact]
        public async Task AuthenticateAsync_should_return_401_when_authentication_fails()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger);
            var model = new AuthenticateDto { EmployeeCardId = this.Fixture.InvalidId, Pin = "1234" };

            // Act
            var response = await controller.AuthenticateAsync(model);

            // Assert
            var result = Assert.IsType<UnauthorizedResult>(response);
        }

        [Fact]
        public async Task AuthenticateAsync_should_return_500_when_error()
        {
            // Arrange
            var controller = new EmployeesController(null, null, this.Logger);
            var model = new AuthenticateDto { EmployeeCardId = this.Fixture.ValidId, Pin = "1234" };

            // Act
            var response = await controller.AuthenticateAsync(model);

            // Assert
            var result = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_should_return_200_with_true_when_updated()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger)
            {
                EmployeeCardId = this.Fixture.ValidId
            };
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal());
            controller.ControllerContext.HttpContext = contextMock.Object;
            var model = new UpdateEmployeeDto();

            // Act
            var response = await controller.UpdateEmployeeAsync(model);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var resultValue  = Boolean.Parse(result.Value.ToString());
            Assert.True(resultValue);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_should_return_200_with_false_when_not_updated()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger)
            {
                EmployeeCardId = this.Fixture.InvalidId
            };
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal());
            controller.ControllerContext.HttpContext = contextMock.Object;
            var model = new UpdateEmployeeDto();

            // Act
            var response = await controller.UpdateEmployeeAsync(model);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var resultValue = Boolean.Parse(result.Value.ToString());
            Assert.False(resultValue);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_should_return_500_when_error()
        {
            // Arrange
            var controller = new EmployeesController(null, null, this.Logger)
            {
                EmployeeCardId = this.Fixture.ValidId
            };
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal());
            controller.ControllerContext.HttpContext = contextMock.Object;
            var model = new UpdateEmployeeDto();

            // Act
            var response = await controller.UpdateEmployeeAsync(model);

            // Assert
            var result = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(500, result.StatusCode);
        }



        [Fact]
        public async Task ChangePinAsync_should_return_200_with_true_when_changed()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger)
            {
                EmployeeCardId = this.Fixture.ValidId
            };
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal());
            controller.ControllerContext.HttpContext = contextMock.Object;
            var model = new ChangePinDto();

            // Act
            var response = await controller.ChangePinAsync(model);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var resultValue = Boolean.Parse(result.Value.ToString());
            Assert.True(resultValue);
        }

        [Fact]
        public async Task ChangePinAsync_should_return_401_when_authentication_fails()
        {
            // Arrange
            var controller = new EmployeesController(this.Fixture.EmployeeRepository, this.Fixture.JwtRepository, this.Logger)
            {
                EmployeeCardId = this.Fixture.InvalidId
            };
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal());
            controller.ControllerContext.HttpContext = contextMock.Object;
            var model = new ChangePinDto();

            // Act
            var response = await controller.ChangePinAsync(model);

            // Assert
            Assert.IsType<UnauthorizedResult>(response);
        }

        [Fact]
        public async Task ChangePinAsync_should_return_500_when_error()
        {
            // Arrange
            var controller = new EmployeesController(null, null, this.Logger)
            {
                EmployeeCardId = this.Fixture.ValidId
            };
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal());
            controller.ControllerContext.HttpContext = contextMock.Object;
            var model = new ChangePinDto();

            // Act
            var response = await controller.ChangePinAsync(model);

            // Assert
            var result = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(500, result.StatusCode);
        }


    }
}
