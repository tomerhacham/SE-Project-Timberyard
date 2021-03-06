using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using WebService.API.ActionFilters;
using WebService.API.Middlewares;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Authentication;
using WebService.Domain.Business.Queries;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Domain.Interface;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region AppConfig
            //App config generation
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            #endregion

            //Dependency injection
            services.Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"))
                    .Configure<AuthenticationSettings>(config.GetSection("AuthenticationSettings"))
                    .Configure<SMPTClientSettings>(config.GetSection("SMPTClientSettings"))
                    .Configure<DefaultSystemAdmin>(config.GetSection("DefaultSystemAdmin"))
                    .AddSingleton<ILogger>(sp => new Logger("Timberyard-service"))
                    .AddSingleton<ISMTPClient, SMTPClient>()
                    .AddSingleton<ILogsAndTestsRepository, LogsAndTestsRepository>()
                    .AddSingleton<IAlarmsAndUsersRepository, AlarmsAndUsersRepository>()
                    .AddSingleton<QueriesController>()
                    .AddSingleton<AlarmsController>()
                    .AddSingleton<AuthenticationController>()
                    .AddSingleton<SystemFacade>();

            services.AddControllers(options => options.Filters.Add(new UnhandledExceptionCheckFilter(new Logger("Timberyard-service"))))
                     .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            //Cors
            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });
            services.AddCors();

            #region Swagger
            //Add Swagger
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Timberyard API",
                    Description = "A simple example ASP.NET Core Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "The Module Group",
                        Email = "TheModule@module.com",
                    }
                });
                c.ExampleFilters();
                c.EnableAnnotations();
                c.IncludeXmlComments(xmlPath);

                if (true)
                {
                    c.AddSecurityDefinition("bearer",
                        new OpenApiSecurityScheme
                        {
                            Description =
                                "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey
                        });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme, Id = "bearer"
                                }
                            },
                            new string[] { }
                        }
                    });
                }


            });
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
            #endregion

            services.AddMvc(config =>
            {
                config.Filters.Add(new ModelStateCheckFilter(new Logger("Timberyard-service")));
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timberyard V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseCors("ClientPermission");


            app.UseRouting();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
