using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notificacao.Domain.Interface;
using Notificacao.Infrastructure.Config;
using Notificacao.Infrastructure.Repository;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;

namespace Notificacao.ConsumerEmail
{
    public class Program
    {
        public static MongoDbContextConfig configMongo;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
               .CreateLogger();

            try
            {
                Log.Information($"Starting {nameof(Notificacao.ConsumerEmail)}");

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (string.IsNullOrEmpty(environment))
                    environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

                string ENV_NAME = environment == "Development" ? "dev" : environment;
                string APP_SETTINGS_ENV = $"appsettings.{ENV_NAME}.json";                           
                
                Log.Warning($"APP_SETTINGS_ENV: {APP_SETTINGS_ENV.ToUpper()}");
                CreateHostBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile($"appsettings.{ENV_NAME}.json", optional: true);
                })
                .Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<MongoDbContextConfig>(hostContext.Configuration.GetSection("MongoDb"));
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IParametroRepository, ParametroRepository>();
                    services.AddSingleton<IEmailService, EmailService>();
                    services.AddSingleton<IKafkaService, KafkaService>();


                    //var db = client.GetDatabase(dataBase);
                    services.AddSingleton<IMongoClient>(provider => new MongoClient(provider.GetRequiredService<IOptions<MongoDbContextConfig>>().Value.ConnectionString));
                    services.AddSingleton<IMongoDatabase>(provider => 
                        provider.GetRequiredService<IMongoClient>().GetDatabase(provider.GetRequiredService<IOptions<MongoDbContextConfig>>().Value.Database)
                    );

                });
    }
}
