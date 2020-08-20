using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;

namespace Vic.Data
{
    /// <summary>
    /// ServiceCollectionExtensions static class, provides extension methods for service collections.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region Variables
        private const string ConnectionStringName = "VicWeb";
        private const string DbUserId = "DB_USERID";
        private const string DbPassword = "DB_PASSWORD";
        #endregion

        #region Methods
        /// <summary>
        /// Add the Victoria database context to the service collection and apply the default configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddVicContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var cs = configuration.GetConnectionString(ConnectionStringName);
            var builder = new MySqlConnectionStringBuilder(cs);
            var user = configuration[DbUserId];
            var pwd = configuration[DbPassword];
            if (!String.IsNullOrEmpty(user))
            {
                builder.UserID = user;
            }
            if (!String.IsNullOrEmpty(pwd))
            {
                builder.Password = pwd;
            }
            services.AddDbContext<VicContext>(options =>
            {
                var sql = options.UseMySql(builder.ConnectionString, o =>
                {
                    o.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
            });

            return services;
        }

        /// <summary>
        /// Add the Victoria database context to the service collection, apply the default configuration and allow for some customization to configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddVicContext(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder> config)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (config == null) throw new ArgumentNullException(nameof(config));

            var cs = configuration.GetConnectionString(ConnectionStringName);
            var builder = new MySqlConnectionStringBuilder(cs);
            var user = configuration[DbUserId];
            var pwd = configuration[DbPassword];
            if (!String.IsNullOrEmpty(user))
            {
                builder.UserID = user;
            }
            if (!String.IsNullOrEmpty(pwd))
            {
                builder.Password = pwd;
            }
            services.AddDbContext<VicContext>(o =>
            {
                var sql = o.UseMySql(builder.ConnectionString, o =>
                {
                    o.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
                config(sql);
            });

            return services;
        }

        /// <summary>
        /// Add the Victoria database context to the service collection, requires manual configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddVicContext(this IServiceCollection services, Action<DbContextOptionsBuilder> config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            services.AddDbContext<VicContext>(o =>
            {
                config(o);
            });

            return services;
        }
        #endregion
    }
}
