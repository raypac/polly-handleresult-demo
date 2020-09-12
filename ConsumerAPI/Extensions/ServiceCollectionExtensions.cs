using System;
using Microsoft.Extensions.DependencyInjection;

namespace ConsumerAPI.Extensions
{
    static class ServiceCollectionExtensions
    {
        //Adds Http Client services
        public static IServiceCollection AddHttpClientDependencies(this IServiceCollection services)
        {
            services.AddHttpClient("FailingApi", option =>
            {
                option.BaseAddress = new Uri("http://localhost:64128/");
            });

            return services;
        }
    }
}
