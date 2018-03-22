using FCMS.Domain.Models;
using FCMS.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FCMS.Tests.FCMS.Integration
{
[Trait("Category", "Integration")]
public class EmployeesIntegrationTests
{
    private TestServer GetServer()
    {
        return new TestServer(WebHost.CreateDefaultBuilder().UseStartup<Startup>());
    }

    private HttpClient GetClient(TestServer server)
    {
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

        private String EmployeeCardId = Guid.NewGuid().ToString();

        [Fact]
        public async Task CreateEmployeeAsync_should_create_employee_and_return_jwt_for_employee()
        {
            // Arrange
            using (var server = this.GetServer())
            using (var client = this.GetClient(server))
            {
                var model = new CreateEmployeeDto
                {
                    EmployeeCardId = this.EmployeeCardId,
                    Email = "integration@test.com",
                    MobileNumber = "0123456789",
                    EmployeeId = Guid.NewGuid().ToString(),
                    Name = "Integration Test",
                    Pin = "1234"
                };

                var json = JsonConvert.SerializeObject(model);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("/api/employees", body);
                response.EnsureSuccessStatusCode();
                var responseString = JsonConvert.DeserializeObject<String>(await response.Content.ReadAsStringAsync());

                // Assert
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(responseString);
                Assert.Equal(model.EmployeeCardId, jwt.Claims.First(x => x.Type == "sub").Value);
            }
        }

        [Fact]
        public async Task UpdateEmployeeAsync_should_be_protected()
        {
            // Arrange
            using (var server = this.GetServer())
            using (var client = this.GetClient(server))
            {
                var model = new UpdateEmployeeDto
                {
                    Email = "integration@test.com",
                    MobileNumber = "0123456789",
                    EmployeeId = Guid.NewGuid().ToString(),
                    Name = "Integration Test",
                };

                var json = JsonConvert.SerializeObject(model);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Act
                var response = await client.PutAsync("/api/employees", body);

                // Assert
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

    [Fact]
    public async Task UpdateEmployeeAsync_should_update_employee_when_using_valid_jwt()
    {
        // Arrange
        using (var server = this.GetServer())
        using (var client = this.GetClient(server))
        {
    
            // Create the employee and get a jwt.
            var model = new CreateEmployeeDto
            {
                EmployeeCardId = Guid.NewGuid().ToString(),
                Email = "integration@test.com",
                MobileNumber = "0123456789",
                EmployeeId = Guid.NewGuid().ToString(),
                Name = "Integration Test",
                Pin = "1234"
            };
            var json = JsonConvert.SerializeObject(model);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/employees", body);
            response.EnsureSuccessStatusCode();
            var jwt = JsonConvert.DeserializeObject<String>(await response.Content.ReadAsStringAsync());
    
            // Create the update definition.
            var updateModel = new UpdateEmployeeDto
            {
                Email = "integration-updated@test.com",
                MobileNumber = "0123456789",
                EmployeeId = Guid.NewGuid().ToString(),
                Name = "Integration Test",
            };
    
            var updateJson = JsonConvert.SerializeObject(model);
            var updateBody = new StringContent(json, Encoding.UTF8, "application/json");
    
            // Add the jwt to the authorization header.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
    
            // Act
            var updateResponse = await client.PutAsync("/api/employees", updateBody);
            response.EnsureSuccessStatusCode();
            var responseString = await updateResponse.Content.ReadAsStringAsync();
    
            // Assert
            var result = Boolean.Parse(responseString);
            Assert.True(result);
        }
    }
    }
}
