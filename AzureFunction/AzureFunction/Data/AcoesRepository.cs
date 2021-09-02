
using AzureFunction.Models;
using MongoDB.Driver;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace AzureFunction.Data
{
    public static class AcoesRepository
    {
        private static readonly MongoClient client;

        static AcoesRepository()
        {
            string connectionString = System.Environment.GetEnvironmentVariable("CosmosDbUrl");
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            client = new MongoClient(settings);
        }

        public static async Task Save(Acao acao)
        {
            try 
            {
                var database = client.GetDatabase("DBAcoes");
                var collection = database.GetCollection<AcaoInDb>("Acoes");

                var acaoInDb = new AcaoInDb
                {
                    Name = acao.Name,
                    Date = acao.Date,
                    Cotacao = acao.Cotacao
                };

                await collection.InsertOneAsync(acaoInDb);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
