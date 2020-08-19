using Fosol.Core.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synology.FileStation;

namespace Synology
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSynologyFileStation(this IServiceCollection services, IConfigurationSection section)
        {
            return services
                .Configure<Options.SynologyOptions>(section)
                .AddHttpClient()
                .AddScoped<IHttpRequestClient, HttpRequestClient>()
                .AddScoped<IFileStationApi, FileStationApi>();
        }
    }
}
