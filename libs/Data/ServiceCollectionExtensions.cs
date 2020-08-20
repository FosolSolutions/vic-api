using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;

namespace Vic.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVicContext(this IServiceCollection services, IConfiguration configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var cs = configure.GetConnectionString("PIMS");
            var builder = new MySqlConnectionStringBuilder(cs);
            var user = configure["DB_USERID"];
            var pwd = configure["DB_PASSWORD"];
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
                var sql = options.UseMySql(builder.ConnectionString);
            });

            return services;
        }

        public static IServiceCollection AddVicContext(this IServiceCollection services, IConfiguration configure, Action<DbContextOptionsBuilder> options)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var cs = configure.GetConnectionString("PIMS");
            var builder = new MySqlConnectionStringBuilder(cs);
            var user = configure["DB_USERID"];
            var pwd = configure["DB_PASSWORD"];
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
                var sql = o.UseMySql(builder.ConnectionString);
                options(sql);
            });

            return services;
        }

        public static IServiceCollection AddVicContext(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            services.AddDbContext<VicContext>(o =>
            {
                options(o);
            });

            return services;
        }
    }
}
