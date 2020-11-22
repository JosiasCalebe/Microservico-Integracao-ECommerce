using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ServiceIntegracao.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ServiceIntegracao
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"------------------------------------------------------------------------");
            Console.WriteLine($"Iniciando Processamento de Encomendas BD --> API / Versão: v.001");
            Console.WriteLine($"------------------------------------------------------------------------");

            CreateHostBuilder(args).Build().Run();
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddCustomConfiguration(hostContext.Configuration);
                    services.AddHostedService<PlataformaWorker>();
                });
    }
}
