// using Binus.WS.Pattern.Entities.Interfaces;
// using Binus.WS.Pattern.Entities.Proxy;

using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ws_with_repository_pattern.Application.AuthorizationRequirement;
using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Middlewares;
using ws_with_repository_pattern.Application.Service;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.DbContext;
using ws_with_repository_pattern.Domain.Repository;
using ws_with_repository_pattern.Infrastructures.Grafana;

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
            
            services.AddSingleton<Instrumentor>();
            services.AddInstrumentorServices();
                
            // fluent validation
            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            
            // DbContext
            services.AddDbContext<KazutoDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("KazutoDB"));
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetSection("ValidUser").Value ?? "",
                    ValidAudience = Configuration.GetSection("ValidAudience").Value ?? "",
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("SecretKey").Value ?? ""))
                };
            });
            
            // Add authorization policies
            services.AddAuthorization(options =>
            {
                // role
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("administrator"));
                options.AddPolicy("UserPolicy", policy =>
                    policy.RequireRole("General"));
                
                // permission
                options.AddPolicy("read", policy =>
                    policy.Requirements.Add(new PermissionRequirement("READ")));

                options.AddPolicy("write", policy =>
                    policy.Requirements.Add(new PermissionRequirement("WRITE")));

                options.AddPolicy("delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("DELETE")));
            });
            
            
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            
            services.AddSingleton<IAuthorizationHandler, Permissionhandler>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
              
            }
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseExceptionMiddleware();
            app.UseInstrumentorApps();

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