using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Synology;
using Vic.Api.Helpers.Authentication;
using Vic.Api.Helpers.Mail;
using Vic.Api.Helpers.Middleware;
using Vic.Data;

namespace Vic.Api
{
    /// <summary>
    /// Startup class, provides a way to start, initialize and configure the API.
    /// </summary>
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
        /// <summary>
        /// Configure dependency injection.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddResponseCaching();
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.IgnoreNullValues = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:IgnoreNullValues"]) ? Boolean.Parse(this.Configuration["Serialization:Json:IgnoreNullValues"]) : false;
                options.PropertyNameCaseInsensitive = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:PropertyNameCaseInsensitive"]) ? Boolean.Parse(this.Configuration["Serialization:Json:PropertyNameCaseInsensitive"]) : false;
                options.PropertyNamingPolicy = this.Configuration["Serialization:Json:PropertyNamingPolicy"] == "CamelCase" ? JsonNamingPolicy.CamelCase : null;
                options.WriteIndented = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:WriteIndented"]) ? Boolean.Parse(this.Configuration["Serialization:Json:WriteIndented"]) : false;
                //options.Converters.Add(new JsonStringEnumConverter());
                //options.Converters.Add(new Int32ToStringJsonConverter());
            });
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:IgnoreNullValues"]) ? Boolean.Parse(this.Configuration["Serialization:Json:IgnoreNullValues"]) : false;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:PropertyNameCaseInsensitive"]) ? Boolean.Parse(this.Configuration["Serialization:Json:PropertyNameCaseInsensitive"]) : false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = this.Configuration["Serialization:Json:PropertyNamingPolicy"] == "CamelCase" ? JsonNamingPolicy.CamelCase : null;
                    options.JsonSerializerOptions.WriteIndented = !String.IsNullOrWhiteSpace(this.Configuration["Serialization:Json:WriteIndented"]) ? Boolean.Parse(this.Configuration["Serialization:Json:WriteIndented"]) : false;
                    //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    //options.JsonSerializerOptions.Converters.Add(new Int32ToStringJsonConverter());
                });
            services.AddSynologyFileStation(this.Configuration.GetSection("Synology"));
            services.Configure<VicAuthenticationOptions>(this.Configuration.GetSection("Authentication"));
            services.Configure<MailOptions>(this.Configuration.GetSection("Mail"));
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddVicContext(this.Configuration, options =>
            {
                if (!this.Environment.IsProduction())
                {
                    var debugLoggerFactory = LoggerFactory.Create(builder => { builder.AddDebug(); });
                    options.UseLoggerFactory(debugLoggerFactory);
                    options.EnableSensitiveDataLogging();
                }
            });
            services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();

            services.AddCors(options =>
            {
                var withOrigins = this.Configuration.GetSection("Cors:WithOrigins").Value.Split(" ");
                options.AddPolicy(name: AllowedOrigins,
                    builder =>
                    {
                        builder
                            .WithOrigins(withOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod(); ;
                    });
            });

            var config = this.Configuration.GetSection("Authentication").Get<VicAuthenticationOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config.Issuer,
                        ValidAudience = config.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                    //options.Events = new JwtBearerEvents()
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        context.Token = context.Request.Cookies[config.Cookie.Name];
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });

            if (!this.Environment.IsProduction())
            {
                // Ignore invalid SSL certificates.
                services.AddHttpClient("HttpRequestClient")
                    .ConfigureHttpClient(config =>
                    {

                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler()
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) =>
                        {
                            return HttpClientHandler.DangerousAcceptAnyServerCertificateValidator(sender, cert, chain, errors);
                        }
                    });
            }

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // TODO: Add to configuration
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue; // TODO: Add to configuration
                options.MultipartBodyLengthLimit = int.MaxValue; // TODO: Add to configuration
            });
        }

        /// <summary>
        /// Configure the application components.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
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

            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        #endregion
    }
}