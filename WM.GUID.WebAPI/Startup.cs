#pragma warning disable 1591
using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WM.GUID.Application.Interfaces;
using WM.GUID.Infrastructure;
using WM.Common;
using WM.GUID.Persistence;
using Swashbuckle.AspNetCore.Swagger;
using WM.Application.GUIDs.Commands.CreateGUID;
using System.Reflection;
using WM.GUID.Application.Infrastructure.Mapping;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using WM.Common.Healthchecks;
using System.IO;
using System;
using WM.GUID.Application.Commands.UpdateGUID;
using WM.GUID.Application.Commands.DeleteGUID;
using WM.GUID.Application.Queries.ReadGUID;

namespace WM.GUID.WebAPI
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
            // Add AutoMapper
            //services.AddAutoMapper(new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "GUID API",
                        Version = "v1",
                        Contact = new Contact { Name = "Health", Url = "/healthchecks-ui" }
                    });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                //c.AddFluentValidationRules();
                //c.OperationFilter<GetUsernameFromHeaderFilter>();
                //c.OperationFilter<GetIsTypescripClientFromHeaderFilter>();
            });



            // Add framework services.
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IDateTime, WMDateTime>();

            // Add MediatR pipeline behaviours
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));


            // Mediatr Register Handlers...
            services.AddMediatR(typeof(CreateGUIDCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(UpdateGUIDCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(DeleteGUIDCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(GetGUIDQueryHandler).GetTypeInfo().Assembly);

            // Add DbContext using SQL Server Provider
            services.AddDbContext<WMDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Database")));

            // Add Redis
            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = Configuration.GetValue<string>("Redis:Name");
                options.Configuration = Configuration.GetValue<string>("Redis:Host");
            });

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //healthchecks
            services.AddHealthChecksUI();
            services.AddHealthChecks()
                .AddCheck<RedisServerHealthCheck>("Redis Health Check")
                .AddCheck<SqlServerHealthCheck>("SQL Health Check");

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<Config>(Configuration.GetSection("ConnectionStrings"));

            //services
            //.AddMvc(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)))
            services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
                //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateGUIDCommandValidator>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GUIDs API v1");
                c.RoutePrefix = "swagger";
            });

            //app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
            app.UseHealthChecksUI();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
