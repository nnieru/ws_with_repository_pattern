using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
namespace ws_with_repository_pattern.Infrastructures.Grafana;


 #region Instrumentor Config
    public sealed class Instrumentor : IDisposable
    {
        #region Instrumentor Enum
        public static class ServiceTypes
        {
            public const string API = "API";
            public const string Scheduler = "Scheduler";
            public const string Subscriber = "Subscriber";

            // define another types here
        }
        #endregion

        public const string ServiceType = ServiceTypes.API;
        public static readonly string ServiceName = Program.Configuration.GetValue<string>("OTel:ServiceName");
        public const string ServiceRuntime = "dotnet";
        public static readonly string OtelHeader = $"Authorization=Basic {Program.Configuration.GetValue<string>("OTel:Key")}";

        public ActivitySource Tracer { get; }
        public Meter Recorder { get; }
        public Counter<long> IncomingRequestCounter { get; }

        public Instrumentor()
        {
            var version = typeof(Instrumentor).Assembly.GetName().Version?.ToString();
            Tracer = new ActivitySource(ServiceName, version);
            Recorder = new Meter(ServiceName, version);
            IncomingRequestCounter = Recorder.CreateCounter<long>("app.incoming.requests",
                description: "The number of incoming requests to the backend API");
        }

        public void Dispose()
        {
            Tracer.Dispose();
            Recorder.Dispose();
        }
    }
    #endregion

    #region Instrumentor Services
    public static class InstrumentorServices
    {
        public static void AddInstrumentorServices(this IServiceCollection service)
        {
            service.AddOpenTelemetry()
                        .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                        .WithMetrics(metrics =>
                          metrics
                            .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                            .AddMeter(Instrumentor.ServiceName)
                            .AddMeter("Microsoft.AspNetCore.Hosting")
                            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                            .AddMeter("Microsoft.AspNetCore.Http.Connections")
                            .AddMeter("Microsoft.AspNetCore.Routing")
                            .AddMeter("Microsoft.AspNetCore.Diagnostics")
                            .AddMeter("Microsoft.AspNetCore.RateLimiting")
                            .AddAspNetCoreInstrumentation() // OpenTelemetry.Instrumentation.AspNetCore package
                            .AddHttpClientInstrumentation() // OpenTelemetry.Instrumentation.Http package
                            .AddRuntimeInstrumentation() // OpenTelemetry.Instrumentation.Runtime package
                            .AddProcessInstrumentation() // OpenTelemetry.Instrumentation.Process package
                            .AddEventCountersInstrumentation(c => // OpenTelemetry.Instrumentation.EventCounters package
                            {
                                // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                                c.AddEventSources(
                                    "Microsoft.AspNetCore.Hosting",
                                    "Microsoft-AspNetCore-Server-Kestrel",
                                    "System.Net.Http",
                                    "System.Net.Sockets");
                            })
                            .AddPrometheusExporter() //OpenTelemetry.Exporter.Prometheus.AspNetCore package
                            .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                            {
                                exporterOptions.Endpoint = new Uri($"{Program.Configuration.GetValue<string>("OTel:Uri")}/v1/metrics");
                                exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                                exporterOptions.Headers = Instrumentor.OtelHeader;
                                exporterOptions.ExportProcessorType = ExportProcessorType.Simple;
                                metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 500;
                            })
                        )
                        .WithTracing(tracerProviderBuilder =>
                            tracerProviderBuilder
                                .AddSource(Instrumentor.ServiceName)
                                .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                                .AddAspNetCoreInstrumentation(opts =>
                                {
                                    opts.Filter = context =>
                                    {
                                        var ignore = new[] { "/swagger" };
                                        return !ignore.Any(s => context.Request.Path.ToString().Contains(s));
                                    };

                                    opts.RecordException = true;
                                })
                                .AddHttpClientInstrumentation(opts =>
                                {
                                    opts.FilterHttpRequestMessage = req =>
                                    {
                                        var ignore = new[] { "/loki/api" };
                                        return !ignore.Any(s => req.RequestUri!.ToString().Contains(s));
                                    };

                                    opts.RecordException = true;
                                })
                                .AddEntityFrameworkCoreInstrumentation(options =>
                                {
                                    options.EnrichWithIDbCommand = (activity, command) =>
                                    {
                                        var stateDisplayName = $"{command.CommandType} main";
                                        activity.DisplayName = stateDisplayName;
                                        activity.SetTag("db.name", stateDisplayName);
                                    };

                                    options.SetDbStatementForText = true;
                                })
                                .AddSqlClientInstrumentation(options => {
                                    options.RecordException = true;
                                    options.SetDbStatementForText = true;
                                })
                                .AddRedisInstrumentation(options => {
                                    options.EnrichActivityWithTimingEvents = true;
                                })
                                .AddProcessor(new Pyroscope.OpenTelemetry.PyroscopeSpanProcessor())
                                .AddOtlpExporter(options =>
                                {
                                    options.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/traces");
                                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                                    options.Headers = Instrumentor.OtelHeader;
                                }));

            service.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
            });

            service.AddEndpointsApiExplorer();
        }
    }
    #endregion

    #region Instrumentor Applications
    public static class InstrumentorApplications
    {
        public static void UseInstrumentorApps(this IApplicationBuilder app)
        {
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
        }
    }
    #endregion

    #region Instrumentor Programs
    public static class InstrumentorPrograms
    {
        public static ServiceProvider LoggerServiceProvider()
        {
            return new ServiceCollection()
                    .AddLogging((loggingBuilder) => loggingBuilder
                        .ClearProviders()
                        .SetMinimumLevel(LogLevel.Debug)
                        .AddOpenTelemetry(logging =>
                        {
                            logging
                                .AddConsoleExporter()
                                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                    .AddService(Instrumentor.ServiceName)
                                    .AddAttributes(new Dictionary<string, object>()
                                    {
                                        ["app"] = Instrumentor.ServiceType,
                                        ["runtime"] = Instrumentor.ServiceRuntime,
                                        ["service.name"] = Instrumentor.ServiceName
                                    }))
                                .AddOtlpExporter(options =>
                                {
                                    options.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/logs");
                                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                                    options.Headers = Instrumentor.OtelHeader;
                                });
                            logging.IncludeFormattedMessage = true;
                            logging.IncludeScopes = true;
                            logging.ParseStateValues = true;
                        }))
                    .BuildServiceProvider();
        }
    }
    #endregion

    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder AddBinusGrafana(this IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddLogging((loggingBuilder) => loggingBuilder
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Information)
                    .AddOpenTelemetry(logging =>
                    {
                        logging
                            .AddConsoleExporter()
                            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                .AddService(Instrumentor.ServiceName)
                                .AddAttributes(new Dictionary<string, object>()
                                {
                                    ["app"] = Instrumentor.ServiceTypes.Scheduler,
                                    ["runtime"] = "dotnet",
                                    ["service.name"] = Instrumentor.ServiceName
                                }))
                            .AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/logs");
                                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                                options.Headers = Instrumentor.OtelHeader;
                            });
                        logging.IncludeFormattedMessage = true;
                        logging.IncludeScopes = true;
                        logging.ParseStateValues = true;
                    }));

                services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                    .WithMetrics(metrics =>
                      metrics
                        .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                        .AddMeter(Instrumentor.ServiceName)
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                        .AddMeter("Microsoft.AspNetCore.Http.Connections")
                        .AddMeter("Microsoft.AspNetCore.Routing")
                        .AddMeter("Microsoft.AspNetCore.Diagnostics")
                        .AddMeter("Microsoft.AspNetCore.RateLimiting")
                        .AddAspNetCoreInstrumentation() // OpenTelemetry.Instrumentation.AspNetCore package
                        .AddHttpClientInstrumentation() // OpenTelemetry.Instrumentation.Http package
                        .AddRuntimeInstrumentation() // OpenTelemetry.Instrumentation.Runtime package
                        .AddProcessInstrumentation() // OpenTelemetry.Instrumentation.Process package
                        .AddEventCountersInstrumentation() // OpenTelemetry.Instrumentation.EventCounters package
                        .AddPrometheusExporter() //OpenTelemetry.Exporter.Prometheus.AspNetCore package
                        .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/metrics");
                            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                            exporterOptions.Headers = Instrumentor.OtelHeader;
                            exporterOptions.ExportProcessorType = ExportProcessorType.Simple;
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 500;
                        })
                    )
                    .WithTracing(tracerProviderBuilder =>
                        tracerProviderBuilder
                            .AddSource(Instrumentor.ServiceName)
                            .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                            .AddAspNetCoreInstrumentation(opts =>
                            {
                                opts.Filter = context =>
                                {
                                    var ignore = new[] { "/swagger" };
                                    return !ignore.Any(s => context.Request.Path.ToString().Contains(s));
                                };

                                opts.RecordException = true;
                            })
                            .AddHttpClientInstrumentation(opts =>
                            {
                                opts.FilterHttpRequestMessage = req =>
                                {
                                    var ignore = new[] { "/loki/api" };
                                    return !ignore.Any(s => req.RequestUri!.ToString().Contains(s));
                                };

                                opts.RecordException = true;
                            })
                            .AddEntityFrameworkCoreInstrumentation(options =>
                            {
                                options.EnrichWithIDbCommand = (activity, command) =>
                                {
                                    var stateDisplayName = $"{command.CommandType} main";
                                    activity.DisplayName = stateDisplayName;
                                    activity.SetTag("db.name", stateDisplayName);
                                };

                                options.SetDbStatementForText = true;
                            })
                            .AddSqlClientInstrumentation(options =>
                            {
                                options.RecordException = true;
                                options.SetDbStatementForText = true;
                            })
                            .AddRedisInstrumentation(options =>
                            {
                                options.EnrichActivityWithTimingEvents = true;
                            })
                            .AddProcessor(new Pyroscope.OpenTelemetry.PyroscopeSpanProcessor())
                            .AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/traces");
                                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                                options.Headers = Instrumentor.OtelHeader;
                            })
                        );

                services.AddHttpLogging(logging =>
                {
                    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
                });
            });


            return builder;
        
        }
    }

    #region IHostBuilder Scheduler
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddBinusGrafana(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddLogging((loggingBuilder) => loggingBuilder
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Information)
                    .AddOpenTelemetry(logging =>
                    {
                        logging
                            .AddConsoleExporter()
                            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                .AddService(Instrumentor.ServiceName)
                                .AddAttributes(new Dictionary<string, object>()
                                {
                                    ["app"] = Instrumentor.ServiceTypes.Scheduler,
                                    ["runtime"] = "dotnet",
                                    ["service.name"] = Instrumentor.ServiceName
                                }))
                            .AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/logs");
                                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                                options.Headers = Instrumentor.OtelHeader;
                            });
                        logging.IncludeFormattedMessage = true;
                        logging.IncludeScopes = true;
                        logging.ParseStateValues = true;
                    }));

                services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                    .WithMetrics(metrics =>
                      metrics
                        .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                        .AddMeter(Instrumentor.ServiceName)
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                        .AddMeter("Microsoft.AspNetCore.Http.Connections")
                        .AddMeter("Microsoft.AspNetCore.Routing")
                        .AddMeter("Microsoft.AspNetCore.Diagnostics")
                        .AddMeter("Microsoft.AspNetCore.RateLimiting")
                        .AddAspNetCoreInstrumentation() // OpenTelemetry.Instrumentation.AspNetCore package
                        .AddHttpClientInstrumentation() // OpenTelemetry.Instrumentation.Http package
                        .AddRuntimeInstrumentation() // OpenTelemetry.Instrumentation.Runtime package
                        .AddProcessInstrumentation() // OpenTelemetry.Instrumentation.Process package
                        .AddEventCountersInstrumentation() // OpenTelemetry.Instrumentation.EventCounters package
                        .AddPrometheusExporter() //OpenTelemetry.Exporter.Prometheus.AspNetCore package
                        .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/metrics");
                            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                            exporterOptions.Headers = Instrumentor.OtelHeader;
                            exporterOptions.ExportProcessorType = ExportProcessorType.Simple;
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 500;
                        })
                    )
                    .WithTracing(tracerProviderBuilder =>
                        tracerProviderBuilder
                            .AddSource(Instrumentor.ServiceName)
                            .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
                            .AddAspNetCoreInstrumentation(opts =>
                            {
                                opts.Filter = context =>
                                {
                                    var ignore = new[] { "/swagger" };
                                    return !ignore.Any(s => context.Request.Path.ToString().Contains(s));
                                };

                                opts.RecordException = true;
                            })
                            .AddHttpClientInstrumentation(opts =>
                            {
                                opts.FilterHttpRequestMessage = req =>
                                {
                                    var ignore = new[] { "/loki/api" };
                                    return !ignore.Any(s => req.RequestUri!.ToString().Contains(s));
                                };

                                opts.RecordException = true;
                            })
                            .AddEntityFrameworkCoreInstrumentation(options =>
                            {
                                options.EnrichWithIDbCommand = (activity, command) =>
                                {
                                    var stateDisplayName = $"{command.CommandType} main";
                                    activity.DisplayName = stateDisplayName;
                                    activity.SetTag("db.name", stateDisplayName);
                                };

                                options.SetDbStatementForText = true;
                            })
                            .AddSqlClientInstrumentation(options =>
                            {
                                options.RecordException = true;
                                options.SetDbStatementForText = true;
                            })
                            .AddRedisInstrumentation(options =>
                            {
                                options.EnrichActivityWithTimingEvents = true;
                            })
                            .AddProcessor(new Pyroscope.OpenTelemetry.PyroscopeSpanProcessor())
                            .AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri($"{ Program.Configuration.GetValue<string>("OTel:Uri") }/v1/traces");
                                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                                options.Headers = Instrumentor.OtelHeader;
                            })
                        );

                services.AddHttpLogging(logging =>
                {
                    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
                });
                // services.AddEndpointsApiExplorer();
            });


            return builder;
        }
    }
#endregion
