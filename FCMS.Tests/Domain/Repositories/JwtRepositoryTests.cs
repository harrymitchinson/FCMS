using FCMS.Mongo;
using FCMS.Mongo.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.Json;
using Xunit;
using FCMS.Domain.Contracts;
using FCMS.Domain.Repositories;
using System.Threading.Tasks;
using FCMS.Mongo.Models;
using MongoDB.Driver;
using AutoMapper;
using FCMS;
using Microsoft.AspNetCore.Identity;
using FCMS.Domain.Models;
using FCMS.Domain.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using FCMS.Models;

namespace FCMS.Tests.Domain.Repositories
{
    [Trait("Category", "Services")]
    public class JwtRepositoryTests : RepositoryTestBase<JwtRepository>
    {
        private IJwtRepository JwtRepository;
        private IOptions<JwtOptions> JwtOptions;
        public JwtRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            this.JwtOptions = Options.Create(configuration.GetSection("Jwt").Get<JwtOptions>());

            this.JwtRepository = new JwtRepository(this.JwtOptions, this.Logger);
        }

        [Fact]
        public void Create_should_return_jwt_for_employee()
        {
            // Arange
            var id = Guid.NewGuid().ToString();
            var model = new EmployeeModel { EmployeeCardId = id };

            // Act
            var result = this.JwtRepository.Create(model);

            // Assert
            Assert.False(String.IsNullOrEmpty(result));

            var handler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.JwtOptions.Value.SecretKey)),
                ValidateLifetime = true,

                ValidateAudience = true,
                ValidAudience = this.JwtOptions.Value.Audience,

                ValidateIssuer = true,
                ValidIssuer = this.JwtOptions.Value.Issuer,
            };

            handler.ValidateToken(result, validationParameters, out SecurityToken validatedToken);

            var jwt = handler.ReadJwtToken(result);
            Assert.Equal(id, jwt.Claims.First(x => x.Type == "sub").Value);
        }

        [Fact]
        public void Validate_should_return_true_when_valid_jwt()
        {
            // Arange
            var id = Guid.NewGuid().ToString();
            var model = new EmployeeModel { EmployeeCardId = id };
            var token = this.JwtRepository.Create(model);

            // Act
            var result = this.JwtRepository.Validate(token);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Validate_should_return_false_when_invalid_jwt()
        {
            // Arange
            var id = Guid.NewGuid().ToString();
            var model = new EmployeeModel { EmployeeCardId = id };
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1MzI5OTQ0Zi1iODRjLTQ1ZWYtOWUzOC03OGRmMzI4ZjBjMWQiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUwMDEiLCJpYXQiOiIxNTE1NTA4MjAwIiwiZXhwIjoiMTUxNTUwODUwMCJ9.ZHKh695thmrJ4MKm1624xDS3eC-7uX9V4-1OopwOHQE";

            // Act
            var result = this.JwtRepository.Validate(token);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_should_throw_when_argument_is_not_jwt()
        {
            // Arange
            var id = Guid.NewGuid().ToString();
            var model = new EmployeeModel { EmployeeCardId = id };
            var token = "Not a token";

            // Act
            var record = Record.Exception(() => this.JwtRepository.Validate(token));

            // Assert
            Assert.NotNull(record);
        }

    }
}
