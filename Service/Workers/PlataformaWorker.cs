using ServiceIntegracao.Plataforma.Service;
using ServiceIntegracao.Data;
using ServiceIntegracao.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Timers;

namespace ServiceIntegracao.Workers
{
    public class PlataformaWorker : WorkerBase<PlataformaWorker>
    {
        private const string MutexName = "PlataformaWorker";
        private readonly ILogger<PlataformaWorker> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IOptionsMonitor<AppSettings> appSettings;
        private readonly DbConnectionFactory dbConnectionFactory;
        private readonly ServiceConfiguration serviceConfiguration;
        private readonly string Plataforma;
        private readonly IConfiguration _config;
        public PlataformaWorker(ILogger<PlataformaWorker> logger,
                            IServiceProvider serviceProvider,
                            IOptionsMonitor<AppSettings> appSettings,
                            DbConnectionFactory dbConnectionFactory,
                            WorkerConfiguration workerConfiguration,
                            IConfiguration _config)
            : base(logger, serviceProvider, MutexName, workerConfiguration)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.appSettings = appSettings;
            this.dbConnectionFactory = dbConnectionFactory;
            this.ElapsedTime += _timer_Elapsed;
            this._config = _config;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var plataformaServices = new PlataformaService(dbConnectionFactory, _config);
            var processo = plataformaServices.ProcessaEncomendasAsync();
            processo.Wait();

        }
    }
}
