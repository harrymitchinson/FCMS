using System;
using System.Text;
using AutoMapper;
using FCMS.Domain.Contracts;
using FCMS.Domain.Options;
using FCMS.Domain.Repositories;
using FCMS.Filters;
using FCMS.Mongo;
using FCMS.Mongo.Models;
using FCMS.Mongo.Options;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FCMS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the auth validation.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = this.GetTokenValidationParameters();
                    options.SaveToken = true;
                });

            // Register MVC services and routing.
            services.AddMvc(options => options.Filters.Add(new ValidateModelFilter()))
            .AddFluentValidation(fv =>
            {
                // Register all the AbstractValidator<T> in this assembly.
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                fv.RegisterValidatorsFromAssembly(typeof(Startup).Assembly);
            });

            // Register AutoMapper for injectable IMapper.
            services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())));

            // Register password hasher for employee pin.
            services.AddSingleton<IPasswordHasher<Employee>, PasswordHasher<Employee>>();

            // Register IOptions and IConfiguration for use.
            services.AddOptions();
            services.Configure<EmployeesContextOptions>(this.Configuration.GetSection("EmployeesContext"));
            services.Configure<JwtOptions>(this.Configuration.GetSection("Jwt"));
            services.AddSingleton(this.Configuration);

            // Register the FCMS services.
            services.AddTransient<EmployeesContext, EmployeesContext>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IJwtRepository, JwtRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
            {
                // Change this for deployment or load from configuration.
                builder.AllowAnyHeader();
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
            });
            app.UseAuthentication();
            app.UseMvc();
        }

        /// <summary>
        /// Gets the token validation parameters using the options from appsettings.json
        /// </summary>
        /// <returns></returns>
        private TokenValidationParameters GetTokenValidationParameters()
        {
            // Setup the auth token validation.
            var secretKey = this.Configuration.GetValue<String>("Jwt:SecretKey");
            var issuer = this.Configuration.GetValue<String>("Jwt:Issuer");
            var audience = this.Configuration.GetValue<String>("Jwt:Audience");
            var tokenValidationParameters = new TokenValidationParameters
            {
                // Check the signing key from config.
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                // Check the token hasn't expired.
                ValidateLifetime = true,
                // Check the audience matches config.
                ValidateAudience = true,
                ValidAudience = audience,
                // Check the issuer matches config.
                ValidateIssuer = true,
                ValidIssuer = issuer
                
            };
            return tokenValidationParameters;
        }
    }
}
