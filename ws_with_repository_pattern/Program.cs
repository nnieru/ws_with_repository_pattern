using System;
using System.IO;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using ws_with_repository_pattern;
using ws_with_repository_pattern.Infrastructures.Grafana;

public class Program
{
    #region -= Properties =-
    public static IConfiguration Configuration { get; set; }
    public static ILogger Logger { get; set; }
    #endregion

    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        Configuration = builder.Build();
        var host = CreateWebHostBuilder(args).Build();
        Logger = InstrumentorPrograms.LoggerServiceProvider()
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<Program>();
        
        host.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            
            .UseUrls(Configuration.GetSection("HostingURL").Value)
            .UseIISIntegration()
            .UseIIS()
            .AddBinusGrafana();
}