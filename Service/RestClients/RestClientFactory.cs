using ServiceIntegracao.Settings;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceIntegracao.RestClients
{
    public class RestClientFactory
    {
        private readonly IConfiguration _config;
        public RestClientFactory(IConfiguration _config)
        {
            this._config = _config;
        }

        public async Task<IRestResponse> RestAPI(string api, Method metodo, Object body)
        {
            var client = new RestClient(this._config.GetValue<string>("API_Access:UrlBase"));
            client.AddDefaultHeader("Authorization", "Bearer " + this._config.GetValue<string>("API_Access:API_Key"));
            var requestFeed = new RestRequest(api, metodo, DataFormat.Json);
            if (body != null)
            {
                requestFeed.AddJsonBody(JsonConvert.SerializeObject(body));
            };
            var result = await client.ExecuteAsync(requestFeed);

            return result;
        }
    }
}
