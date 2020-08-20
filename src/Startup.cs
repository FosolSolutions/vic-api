using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Synology;
using Vic.Api.Helpers.Middleware;
using Vic.Data;

namespace Vic.Api
{
    public class Startup
    {
        #region Variables
        private const string AllowedOrigins = "allowedOrigins";
        #endregion

        #region Properties
        /// <summary>
        /// get - The application configuration settings.
        /// </summary>
        /// <value></value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// get/set - The environment settings for the application.
        /// </summary>
        /// <value></value>
        public IWebHostEnvironment Environment { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instances of a Startup class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.Configuration = configuration;
            this.Environment = env;
        }
        #endregion

        #region Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers();
            services.AddSynologyFileStation(this.Configuration.GetSection("Synology"));
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.IgnoreNullValues = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:IgnoreNullValues"]) ? Boolean.Parse(this.Configuration["Serialization:Json:IgnoreNullValues"]) : false;
                options.PropertyNameCaseInsensitive = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:PropertyNameCaseInsensitive"]) ? Boolean.Parse(this.Configuration["Serialization:Json:PropertyNameCaseInsensitive"]) : false;
                options.PropertyNamingPolicy = this.Configuration["Serialization:Json:PropertyNamingPolicy"] == "CamelCase" ? JsonNamingPolicy.CamelCase : null;
                options.WriteIndented = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:WriteIndented"]) ? Boolean.Parse(this.Configuration["Serialization:Json:WriteIndented"]) : false;
                //options.Converters.Add(new JsonStringEnumConverter());
                //options.Converters.Add(new Int32ToStringJsonConverter());
            });
            services.AddVicContext(this.Configuration, options =>
            {
                if (!this.Environment.IsProduction())
                {
                    var debugLoggerFactory = LoggerFactory.Create(builder => { builder.AddDebug(); });
                    options.UseLoggerFactory(debugLoggerFactory);
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowedOrigins,
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:3000", "https://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod(); ;
                    });
            });

            if (this.Environment.IsDevelopment())
            {
                // Ignore invalid SSL certificates.
                services.AddHttpClient("HttpRequestClient")
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler()
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseRouting();
            app.UseCors(AllowedOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        #endregion
    }
}
