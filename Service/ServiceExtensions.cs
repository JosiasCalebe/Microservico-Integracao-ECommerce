using ServiceIntegracao.Data;

using ServiceIntegracao.Settings;
using ServiceIntegracao.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceIntegracao
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient((serviceProvider) => DbConnectionFactory.Create(configuration.GetConnectionString("Default")));       
            services.Configure<AppSettings>(configuration);

            //var configService = new ServiceConfiguration();

            //services.AddTransient<ServiceConfiguration>((serviceProvider) => { return configService; });

            var workerConfig = new WorkerConfiguration
            {
                WorkerConfigs = new List<WorkerConfig>()
            };

            ConfigurationBinder.Bind(configuration, "WorkerConfiguration", workerConfig.WorkerConfigs);
            services.AddTransient((serviceProvider) => { return workerConfig; });

            services.AddScoped(provider => provider.GetService<IOptionsMonitor<AppSettings>>().CurrentValue);

            return services;
        }
    }
}
