using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ServiceIntegracao.Logger.Service
{
    public class LogService
    {
        public async Task LogGerarArquivoAsync(string fileName, string diretorio, object objeto)
        {
            try
            {
                string data = DateTime.Now.ToString("yyyy-MM-dd");

                string dir = $"{Directory.GetCurrentDirectory()}\\LogServices\\{data}\\{diretorio}";


                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                };

                string arquivo = $"{dir}\\{fileName}.json";

                await File.WriteAllTextAsync(arquivo, JsonConvert.SerializeObject(objeto, Formatting.Indented));
            }
            catch (Exception e)
            {
                string log = $"Erro ao Gerar Arquivo de Log - {e.Message} - {DateTime.Now}";

                Console.WriteLine(log);
            }
        }
    }
}