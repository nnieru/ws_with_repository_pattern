// using Binus.WS.Pattern.Entities.Interfaces;
// using Binus.WS.Pattern.Entities.Proxy;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Service;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.DbContext;
using ws_with_repository_pattern.Domain.Repository;

namespace ws_with_repository_pattern
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
            
            // DbContext
            services.AddDbContext<KazutoDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("KazutoDB"));
            });
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            // DI
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
              
            }
            
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