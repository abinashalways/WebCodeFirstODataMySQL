
using Microsoft.EntityFrameworkCore;
using WebCodeFirstODataMySQL.Database_Context;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder.Conventions;
using Microsoft.OData.ModelBuilder;
using WebCodeFirstODataMySQL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using WebCodeFirstODataMySQL.Repository;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using WebCodeFirstODataMySQL.Service;
using System.Reflection.Emit;


namespace WebCodeFirstODataMySQL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var modelbuilder = new ODataConventionModelBuilder();
            modelbuilder.EntitySet<Employee>("Empdetails");
            modelbuilder.EntitySet<Department>("Department");
             modelbuilder.EntitySet<Location>("Location");


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My OData API", Version = "v1" });

                
                c.OperationFilter<ODataOperationFilter>();
                
            });
            builder.Services.AddControllers()
    .AddOData(options => options.Select().Expand().Filter().OrderBy().Count().SetMaxTop(100)
    .AddRouteComponents("odata", modelbuilder.GetEdmModel()));

            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddDbContext<EmpDetailsContext>(options => 
            options.UseMySql(builder.Configuration.GetConnectionString("EmpConnection"), new MySqlServerVersion(new Version(8, 0, 39))).EnableSensitiveDataLogging().LogTo(Console.WriteLine,LogLevel.Information)
            );
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("4cCI6MTcyNDQyNzE0OCwiaWF0IjoxNzI0NDIzNTQ4fQ.LeVX7Z7__frSIH7vUuYUCInJ2aYZCc8A2GvS1NecIak")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });

            builder.Services.AddAuthorization();


            //modelbuilder.Services.AddControllers().AddOData(options =>
            //{
            //    var modelBuilder = new ODataConventionModelBuilder();
            //    modelBuilder.EntitySet<Employee>("Empdetails");
            //    modelBuilder.EntitySet<Department>("Department");
            //    modelBuilder.EntitySet<Location>("Location");
                
            //    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
            //        .AddRouteComponents("odata", modelBuilder.GetEdmModel());
            //});

            // Add services to the container.

            builder.Services.AddControllers()
                      .AddJsonOptions(options =>
                      {
                          options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                      });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.ResolveConflictingActions(apiDescription => apiDescription.First());
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        In = ParameterLocation.Header,
            //        Description = "Please enter token in the format: Bearer {token}",
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey
            //    });
            //    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //        {
            //            {
            //                new OpenApiSecurityScheme
            //                {
            //                    Reference = new OpenApiReference
            //                    {
            //                        Type = ReferenceType.SecurityScheme,
            //                        Id = "Bearer"
            //                    }
            //                },
            //                Array.Empty<string>()
            //            }
            //        });
            //});
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My OData API v1");
               
            });
            //}

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }

}