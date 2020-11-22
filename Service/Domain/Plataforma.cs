using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceIntegracao.Domain
{
    public class RegistraColetaRequest
    {
        public string CodRemessa { get; set; }
        public List<Encomenda> Encomendas { get; set; }
    }
    public class Encomenda
    {
        public string TipoServico { get; set; }
        public string TipoServicoTipo { get; set; }
        public string TipoEntrega { get; set; }
        public string Peso { get; set; }
        public string Volumes { get; set; }
        public string CondFrete { get; set; }
        public string Pedido { get; set; }
        public string IdCliente { get; set; }
        public string Natureza { get; set; }
        public string TipoVolumes { get; set; }
        public string IsencaoIcms { get; set; }
        public string InfoColeta { get; set; }
        public string DestNome { get; set; }
        public string DestCpfCnpj { get; set; }
        public string DestIe { get; set; }
        public string DestEnd { get; set; }
        public string DestEndNum { get; set; }
        public string DestCompl { get; set; }
        public string DestPontoRef { get; set; }
        public string DestBairro { get; set; }
        public string DestCidade { get; set; }
        public string DestEstado { get; set; }
        public string DestPais { get; set; }
        public string DestCep { get; set; }
        public string DestEmail { get; set; }
        public string DestDdd { get; set; }
        public string DestTelefone1{ get; set; }
        public string DestTelefone2 { get; set; }
        public string DestTelefone3 { get; set; }
        public string Campanha { get; set; }
        public Cod Cod { get; set; }
        public Agendamento Agendamento { get; set; }
        public DocFiscalNFe DocFiscalNFe { get; set; }
        public DocFiscalNF DocFiscalNF { get; set; }
        public DocFiscalO DocFiscalO { get; set; }
        public string CNPJ { get; set; }
    }

    public class Cod
    {
        public string FormaPagto { get; set; }
        public string Parcelas { get; set; }
        public string Valor { get; set; }
    }
    public class Agendamento
    {
        public string AgData { get; set; }
        public string AgPeriodo1 { get; set; }
        public string AgPeriodo2 { get; set; }
    }
    public class DocFiscalNFe
    {
        public string NfeNumero { get; set; }
        public string NfeSerie { get; set; }
        public string NfeData { get; set; }
        public string NfeValTotal { get; set; }
        public string NfeValProd { get; set; }
        public string NfeCfop { get; set; }
        public string NfeChave { get; set; }
    }

    public class DocFiscalNF
    {
        public string NfNumero { get; set; }
        public string NfSerie { get; set; }
        public string NfData { get; set; }
        public string NfValTotal { get; set; }
        public string NfValBc { get; set; }
        public string NfValIcms { get; set; }
        public string NfValBcSt { get; set; }
        public string NfValIcmsSt { get; set; }
        public string NfValProd { get; set; }
        public string NfCfop { get; set; }
    }

    public class DocFiscalO
    {
        public string NfoTipo { get; set; }
        public string NfoDescricao { get; set; }
        public string NfoNumero { get; set; }
        public string NfoData { get; set; }
        public string NfoValValTotal { get; set; }
        public string NfoValProd { get; set; }
        public string NfoCfop { get; set; }
    }
}
