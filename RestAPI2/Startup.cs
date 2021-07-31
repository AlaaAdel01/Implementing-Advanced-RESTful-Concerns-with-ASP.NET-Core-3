using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using RestAPI2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
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

            services.AddHttpCacheHeaders((expirationModelOptions)=>
                {
                    expirationModelOptions.MaxAge = 50;
                    expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
                 },
                 (validationModelOptions)=>
                 {
                     validationModelOptions.MustRevalidate = true;
                 }
                 );
                
                services.AddResponseCaching();
            services.AddControllers(
                setupAction => {
                    setupAction.ReturnHttpNotAcceptable = true;
                }).AddNewtonsoftJson(setupAction=>
                {
                    setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetailsFactory = context.HttpContext.RequestServices
                       .GetRequiredService<ProblemDetailsFactory>();

                        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext,context.ModelState);

                        problemDetails.Detail = "see fail error";
                        problemDetails.Instance = context.HttpContext.Request.Path;


                        var actionExecutionContext=context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
                        if ((context.ModelState.ErrorCount>0 )&&(actionExecutionContext?.ActionArguments.Count==context.ActionDescriptor.Parameters.Count))
                        {
                            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                            problemDetails.Title = "one or more errors are occured";
                        }

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };

                        problemDetails.Status = StatusCodes.Status400BadRequest;
                        problemDetails.Title = "one or more inputs are invalid";
                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                };
                 });

            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
            if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateos+json");
                }
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IPropertyCheckerService,PropertyCheckerService>();
            services.AddTransient<IPropertyMappingServices, PropertyMappingServices>();
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (env.IsProduction())
            {
                app.UseExceptionHandler(appBuilder =>
                appBuilder.Run(async context => { context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An un hadelled fault happened");
                })
                ) ;
            }

            app.UseHttpsRedirection();
            app.UseResponseCaching();


            app.UseHttpCacheHeaders();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoint=>endpoint.MapControllers());
        }
    }
}
