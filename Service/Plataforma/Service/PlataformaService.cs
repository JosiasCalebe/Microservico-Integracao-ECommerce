using ServiceIntegracao.Domain;
using ServiceIntegracao.Logger.Service;
using ServiceIntegracao.Plataforma.Repository;
using ServiceIntegracao.Data;
using ServiceIntegracao.RestClients;
using ServiceIntegracao.Settings;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ServiceIntegracao.Plataforma.Service
{
    public class PlataformaService
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        private const string PostUrl = " ";
        private const string GetUrl = " ";
        private readonly IConfiguration _config;

        public PlataformaService(DbConnectionFactory dbConnectionFactory, IConfiguration _config)
        {
            this.dbConnectionFactory = dbConnectionFactory;
            this._config = _config;
        }

        private string GerarTrackingUrl(Encomenda ped)
        {
            string trackingUrl = @$"https://plataforma.com/tracker?reid={_config.GetValue<string>("Reid")}&pedido={ped.Pedido}&nfiscal={ped.DocFiscalNFe.NfeNumero}";
            return trackingUrl;
        }

        private async Task GerarLog(Encomenda encomenda, string message)
        {
            LogService logService = new LogService();
            bool geraLog = _config.GetValue<bool>("GeraArquivoLog");
            if (geraLog)
            {
                string fileName = $"{encomenda.Pedido}";
                var log = new RetornoLog();
                log.acao = $"PostEncomendaPlataforma";
                log.message = message;
                log.request = encomenda;
                log.response = message;

                await logService.LogGerarArquivoAsync(fileName, "Plataforma", log);
                string retorno = $"OrderId: {encomenda.Pedido} - Nao Processado! - ({message}) - {DateTime.Now}";
                Console.WriteLine(retorno);
            }
        }

        public async Task PostPedidosAsync(Encomenda encomenda, PlataformaRepository plataformaRepository)
        {
            RestClientFactory restClientFactory = new RestClientFactory(this._config);
            try
            {
                IRestResponse response = await restClientFactory.RestAPI("newOrder", Method.POST, encomenda);
                if (response.IsSuccessful)
                {
                    await plataformaRepository.CommitPedidosAsync(encomenda.Pedido, encomenda.DocFiscalNFe.NfeNumero, GerarTrackingUrl(encomenda));
                    string retorno = $"OrderId: {encomenda.Pedido} - Incluido.";
                    Console.WriteLine(retorno);
                }
                else 
                {
                    await GerarLog(encomenda, response.Content);
                }
            }
            catch (Exception e)
            {
                await GerarLog(encomenda, e.Message);
            }
        }

        public async Task ProcessaEncomendasAsync()
        {
            using (var conn = dbConnectionFactory.Create())
            {
                var plataformaRepository = new PlataformaRepository(conn, this._config);
                var logService = new LogService();
                bool geraLog = _config.GetValue<bool>("GeraArquivoLog");
                var listPedidos = await plataformaRepository.GetPedidosAsync();
                var totalPedidos = listPedidos.Count;

                Console.WriteLine($"-------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"Processando Encomendas - ({totalPedidos})  - {DateTime.Now}");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------------");

                if (listPedidos.Count > 0)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    foreach (var pedido in listPedidos)
                    {
                        try
                        {
                            bool status = await plataformaRepository.CheckPedidoAsync(pedido.Pedido);
                            //teste();
                            //Console.WriteLine(GerarTrackingUrl(pedido));
                            if (status) await PostPedidosAsync(pedido, plataformaRepository);
                            else
                                Console.WriteLine($"Pedido já cadastrado - OrderId: {pedido.Pedido}");
                        }
                        catch (Exception e)
                        {
                            string retorno = $"Erro OrderId: {pedido.Pedido} - ({e.Message}) - {DateTime.Now}.";
                            Console.WriteLine(retorno);

                            await plataformaRepository.PedidoLogAsync(pedido.Pedido, retorno);

                            if (geraLog)
                            {
                                string fileName = $"{pedido.Pedido}";
                                var log = new RetornoLog();
                                log.acao = $"PostEncomendaPlataforma";
                                log.message = retorno;
                                log.request = pedido;
                                log.response = e.Message;

                                await logService.LogGerarArquivoAsync(fileName, "Plataforma", log);
                            }
                        }
                    }

                    sw.Stop();

                    Console.WriteLine($"Processamento de Pedidos da Plataforma Concluido com Sucesso. Total ({totalPedidos}) - {DateTime.Now} - ({sw.ElapsedMilliseconds / 1000}s)");
                }
            }
        }

    }
}
