using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spo.GraphApi.Models;

namespace Spo.GraphApi;

public static class GraphApiServiceCollectionExtensions
{
    public static IServiceCollection AddGraphApiServices(
         this IServiceCollection services, IConfiguration config)
    {
        services.Configure<GraphApiOptions>(
            config.GetSection(GraphApiOptions.GraphApiSettings));
        services.AddScoped<IGraphApiClientFactory, GraphApiClientFactory>();
        return services;
    }
}


