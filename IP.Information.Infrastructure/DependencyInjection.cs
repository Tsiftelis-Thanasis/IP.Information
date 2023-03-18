using IP.Information.Application.BackgroundServices;
using IP.Information.Application.Interfaces;
using IP.Information.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IP.Information.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddContosoServices(this IServiceCollection serviceCollection)
        {
            _ = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddSingleton<ICachingIPAddresses, CachingIPAddresses>();
            serviceCollection.AddSingleton<IIPAddressStore, IPAddressStore>();
            serviceCollection.AddTransient(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            serviceCollection.AddTransient<IConnectionService, ConnectionService>();
            serviceCollection.AddSingleton<IHostedService, DBInfoUpdater>();

            return serviceCollection;
        }
    }
}