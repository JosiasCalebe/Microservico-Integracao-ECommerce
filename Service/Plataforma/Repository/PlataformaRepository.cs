using ServiceIntegracao.Domain;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceIntegracao.Plataforma.Repository
{
    public class PlataformaRepository
    {
        private readonly SqlConnection connection;
        private readonly IConfiguration _config;

        public PlataformaRepository(SqlConnection connection, IConfiguration _config)
        {
            this.connection = connection;
            this._config = _config;
        }

        private async Task<List<dynamic>> PedidosAsync()
        {
            string sql = $"SELECT DISTINCT p.idPedido as Pedido, p.Cliente as DestNome, c.Endereco as DestEnd, c.Numero  as DestEndNum, c.Complemento  as DestCompl, c.Referencia  as DestPontoRef, c.Bairro as DestBairro, c.Cidade as DestCidade, c.UF as DestEstado,c.CEP  as DestCep, c.PAIS as DestPais, c.IE as DestIe, c.Documento as DestCpfCnpj, c.Email as DestEmail, c.Telefone as DestTelefone1, e.Serie as NFeSerie, e.Nf as NFeNumero, e.Emissao as NFeData, e.ChaveNfe as NFeChave, e.TotalNf as NFeValTotal, p.TotalReferencia as NFeValProd, l.FriendlyName As Transportadora" +
                $" FROM Pedidos AS p with(nolock)" +
                $" INNER JOIN PedidosCliente AS c with(nolock) ON c.idPedido = p.idPedido" +
                $" INNER JOIN PedidosNfe AS e with(nolock) ON e.idPedido = p.idPedido" +
                $" INNER JOIN PedidosLogistica as l with(nolock) ON l.idPedido = p.idPedido" +
                $" WHERE e.ChaveNfe IS NOT NULL" +
                $" AND e.Nf IS NOT NULL" +
                $" AND e.Emissao IS NOT NULL" +
                $" AND l.Encerrado = 0" +
                $" AND l.FriendlyName LIKE '%{this._config.GetValue<string>("Transportadora")}%'";

            var pedido = await connection.QueryAsync<dynamic>(sql, commandTimeout: 0);
            return pedido.ToList();
        }

        public async Task<List<Encomenda>> GetPedidosAsync()
        {
            List<Encomenda> encomendas = new List<Encomenda>();
            try
            {
                var pedidosLista = await PedidosAsync();
                List<string> tiposServico = _config.GetSection("tiposServico").Get<List<string>>();
                int i;
                foreach (var ped in pedidosLista)
                {
                    Stopwatch stop = new Stopwatch();
                    stop.Start();
                    Encomenda encomenda = new Encomenda();
                    DocFiscalNFe NFe = new DocFiscalNFe();
                    i = 1;
                    foreach (string s in tiposServico)
                    {
                        if (ped.Transportadora.Contains(s))
                        {
                            encomenda.TipoServico = i.ToString();
                            break;
                        }
                        i++;
                    }
                    encomenda.TipoEntrega = _config.GetValue<string>("TipoEntrega");
                    encomenda.Volumes = _config.GetValue<string>("Volumes");
                    encomenda.CondFrete = _config.GetValue<string>("CondFrete");
                    encomenda.Pedido = ped.Pedido.ToString();
                    encomenda.Natureza = _config.GetValue<string>("Natureza");
                    encomenda.IsencaoIcms = _config.GetValue<string>("IsencaoIcms");
                    encomenda.DestNome = ped.DestNome;
                    encomenda.DestCpfCnpj = ped.DestCpfCnpj;
                    encomenda.DestIe = ped.DestIe;
                    encomenda.DestEnd = ped.DestEnd;
                    encomenda.DestEndNum = ped.DestEndNum;
                    encomenda.DestCompl = ped.DestCompl;
                    encomenda.DestPontoRef = ped.DestPontoRef;
                    encomenda.DestBairro = ped.DestBairro;
                    encomenda.DestCidade = ped.DestCidade;
                    encomenda.DestEstado = ped.DestEstado;
                    encomenda.DestPais = ped.DestPais;
                    encomenda.DestCep = ped.DestCep;
                    encomenda.DestEmail = ped.DestEmail;
                    encomenda.DestTelefone1 = ped.DestTelefone1.Replace($"+55", "");
                    NFe.NfeSerie = ped.NFeSerie;
                    NFe.NfeNumero = ped.NFeNumero;
                    NFe.NfeData = ped.NFeData.ToString();
                    NFe.NfeValTotal = ped.NFeValTotal.ToString();
                    NFe.NfeValProd = ped.NFeValProd.ToString();
                    NFe.NfeChave = ped.NFeChave;
                    encomenda.DocFiscalNFe = NFe;

                    encomendas.Add(encomenda);
                    //if (i == 1) break;
                }
                return encomendas;
            }
            catch(Exception)
            {
                return null;
            }   
        }

        public async Task CommitPedidosAsync(string idPedido, string objetoRastreio, string urlRastreio)
        {
            try
            {
                string sql = $"UPDATE PedidosLogistica SET " +
                    $"ObjetoRastreio = '{objetoRastreio}', " +
                    $"UrlRastreio = '{urlRastreio}', " +
                    $"DataAlteracao = getdate(), " +
                    $"encerrado = 1 " +
                $"WHERE idPedido = {idPedido}";

                await connection.ExecuteAsync(sql, commandTimeout: 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao Efetuar o Commit Pedido: {idPedido} - ({e.Message}) - {DateTime.Now}");
            }
        }

        public async Task<bool> CheckPedidoAsync(string OrderId)
        {
            try
            {
                string sql = $"SELECT * FROM PedidosLogistica " +
                    $"WHERE idPedido = '{OrderId}' ";
                var result = await connection.QueryFirstOrDefaultAsync(sql, commandTimeout: 0);
                if (result.UrlRastreio == null) return true;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao Efetuar a checagem do Pedido: {OrderId} - ({e.Message}) - {DateTime.Now}");
                return false;
            }
        }

        public async Task PedidoLogAsync(string OrderId, string message)
        {
            try
            {
                var logData = new DynamicParameters();
                logData.Add("@ORDERID", OrderId);
                logData.Add("@VALOR", message);
                logData.Add("@DATA_ALTERACAO", DateTime.Now);

                await connection.ExecuteAsync("stpPedidosLog", logData, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 0);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao gerar log Pedido: {OrderId} - ({e.Message}) - {DateTime.Now}");
            }

        }
    }
}
