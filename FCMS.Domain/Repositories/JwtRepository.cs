using FCMS.Domain.Contracts;
using FCMS.Mongo;
using FCMS.Mongo.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using FCMS.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using FCMS.Domain.Options;
using Microsoft.IdentityModel.Tokens;

namespace FCMS.Domain.Repositories
{
    public class JwtRepository : RepositoryBase<JwtRepository>, IJwtRepository
    {
        private IOptions<JwtOptions> JwtOptions { get; }
        private JwtHeader JwtHeader { get; }
        private JwtSecurityTokenHandler JwtSecurityTokenHandler { get; }
        private SecurityKey Key { get; }
        public JwtRepository(IOptions<JwtOptions> options, ILogger<JwtRepository> logger) : base(logger)
        {
            this.JwtOptions = options;

            this.JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            this.Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.JwtOptions.Value.SecretKey));

            var signingCredentials = new SigningCredentials(this.Key, SecurityAlgorithms.HmacSha256);
            this.JwtHeader = new JwtHeader(signingCredentials);
        }

        public String Create(IEmployeeModel model)
        {
            try
            {
                // Get the time now for reference.
                var nowUtc = DateTime.UtcNow;

                // Get the time the token should expire.
                var expires = nowUtc.AddMinutes(this.JwtOptions.Value.ExpiryMinutes);

                // Get the expiry and issue time in seconds.
                var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
                var expireAt = (Int64)(new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds);
                var issuedAtTime = (Int64)(new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);

                // Create the token claims.
                var claims = new List<Claim>
                {
                    new Claim("sub", model.EmployeeCardId),
                    new Claim("iss", this.JwtOptions.Value.Issuer),
                    new Claim("aud", this.JwtOptions.Value.Audience),
                    new Claim("iat", issuedAtTime.ToString()),
                    new Claim("exp", expireAt.ToString())
                };

                // Create the token.
                var payload = new JwtPayload(claims);
                var jwt = new JwtSecurityToken(this.JwtHeader, payload);
                var token = this.JwtSecurityTokenHandler.WriteToken(jwt);

                // Return the token.
                return token;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error creating token for employee card id {0}", model.EmployeeCardId);
                throw;
            }
        }

        public Boolean IsToken(String token)
        {
            return this.JwtSecurityTokenHandler.CanReadToken(token);
        }

        public Boolean Validate(String token)
        {
            try
            {
                // Setup the validation parameters.
                var validationParameters = new TokenValidationParameters
                {
                    // Check the signing key from config.
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.JwtOptions.Value.SecretKey)),
                    // Check the token hasn't expired.
                    ValidateLifetime = true,
                    // Check the audience matches config.
                    ValidateAudience = true,
                    ValidAudience = this.JwtOptions.Value.Audience,
                    // Check the issuer matches config.
                    ValidateIssuer = true,
                    ValidIssuer = this.JwtOptions.Value.Issuer,
                };
                // Validate the token using our parameters.
                this.JwtSecurityTokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Return true if validatedToken is anything.
                return validatedToken != null;
            }
            catch (SecurityTokenException)
            {
                // If validation fails we need to return false.
                return false;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error validating token {0}", token);
                throw;
            }
        }
    }
}
