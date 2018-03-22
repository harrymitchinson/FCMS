using FCMS.Models;
using FCMS.Validation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FCMS.Tests.FCMS.Integration
{
    [Trait("Category", "Integration")]
    public class ValidationFilterTests
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

        [Fact]
        public async Task ValidationFilter_should_catch_invalid_models()
        {
            // Arrange
            using (var server = this.GetServer())
            using (var client = this.GetClient(server))
            {
                var model = new CreateEmployeeDto();
                var json = JsonConvert.SerializeObject(model);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("/api/employees", body);
                var result = JsonConvert.DeserializeObject<ValidationResultModel>(await response.Content.ReadAsStringAsync());

                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.False(String.IsNullOrEmpty(result.Message));
                // Model can change however we will always need at least the employeeCardId
                Assert.True(result.Errors.Count > 1);
            }
        }
    }
}
