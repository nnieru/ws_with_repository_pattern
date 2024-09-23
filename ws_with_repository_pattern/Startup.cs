using Binus.WS.Pattern.Entities.Interfaces;
using Binus.WS.Pattern.Entities.Proxy;
using Binus.WS.Pattern.RouteGuard.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSG7.C2.Student.Activity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddControllers();
            services.AddMvcCore().AddApiExplorer();

            // Inject here
            services.AddScoped<IEntityHelper, EntityHelperProxy>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "test api",
                    Description = "A simple example ASP.NET Core Web API",
                });


                // Set the comments path for the Swagger JSON and UI.
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);


                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            // Add CORS policy for Dev environment
            services.AddCors(options =>
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            ));

            // Enable GZIP Compression
            services.Configure<GzipCompressionProviderOptions>(opt => opt.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage()
                    .UseSwagger() // Enable middleware to serve generated Swagger as a JSON endpoint.
                    .UseSwaggerUI(c =>
                    {
                        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                        // specifying the Swagger JSON endpoint.
                        c.SwaggerEndpoint("v1/swagger.json", "API.Test API V1");
                    })
                    .UseCors("AllowAll");
            }

            //FileHelper.Initialize(env);

            // app.UseCors(
            //      options => options.WithOrigins("https://dev-sa.apps.binus.ac.id", "https://studentactivity.apps.binus.ac.id").AllowAnyHeader().AllowAnyMethod()
            //  );
            // app.UseCors();

            app.UseRouting();

            // app.UseMiddleware<RouteGuardMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("BINUS V3.0");
            });


        }
    }
}