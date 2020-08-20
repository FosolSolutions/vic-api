using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using MySql.Data.MySqlClient;
using System.IO;

namespace Vic.Data
{
    /// <summary>
    /// VicContextFactory sealed class, provides a way to integrate 'dotnet ef' with the database.
    /// </summary>
    public sealed class VicContextFactory : IDesignTimeDbContextFactory<VicContext>
    {
        #region Variables
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a VicContextFactory class.
        /// </summary>
        public VicContextFactory()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Vic.Api", LogLevel.Debug)
                    .AddConsole();
            });
            _logger = loggerFactory.CreateLogger<VicContextFactory>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create the database context so that the design time tools can connect to it.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public VicContext CreateDbContext(string[] args)
        {
            DotNetEnv.Env.Load();
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            // As per Microsoft documentation, a typical sequence of configuration providers is:
            //   1. appsettings.json
            //   2. appsettings.{Environment}.json
            //   3. Secret Manager (if in development)
            //   4. Environment variables using the Environment Variables configuration provider.
            //   5. Command - line arguments using the Command-line configuration provider.
            // source: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#configuration-providers
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("connectionstrings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"connectionstrings.{environment}.json", optional: true, reloadOnChange: true);

            if (environment != null && !environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
            {
                builder.AddUserSecrets<VicContext>();
            }

            builder.AddEnvironmentVariables();

            var config = builder.Build();

            var cs = config.GetConnectionString("VicWeb");

            _logger.LogInformation($"Creating database context for '{environment}':'{cs}'.");

            var sqlBuilder = new MySqlConnectionStringBuilder(cs)
            {
                UserID = config["DB_USERID"],
                Password = config["DB_PASSWORD"]
            };

            var optionsBuilder = new DbContextOptionsBuilder<VicContext>();
            optionsBuilder.UseMySql(sqlBuilder.ConnectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));
            return new VicContext(optionsBuilder.Options);
        }
        #endregion
    }
}
