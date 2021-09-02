using Azure.Messaging.EventHubs.Consumer;
using KafkaConsumerHub.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumerHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bootstrapServer = ConfigurationManager.AppSettings.Get("bootstrapServers");
            var topicName = ConfigurationManager.AppSettings.Get("topicName");
            var password = ConfigurationManager.AppSettings.Get("password");

            var consumer = new EventHubConsumerClient(
                "grupo1",
                password,
                topicName);

            try
            {
                Console.WriteLine("Grupo: ");
                Console.WriteLine(args[0]);

                using CancellationTokenSource cancellationSource = new CancellationTokenSource();
                //cancellationSource.CancelAfter(TimeSpan.FromSeconds(200));

                await foreach (PartitionEvent partitionEvent in consumer.ReadEventsFromPartitionAsync(args[0],EventPosition.Latest ,cancellationSource.Token))
                {
                    string readFromPartition = partitionEvent.Partition.PartitionId;
                    var eventBody = partitionEvent.Data.EventBody;

                    Acao acao = JsonConvert.DeserializeObject<Acao>(partitionEvent.Data.EventBody.ToString());

                    Console.WriteLine($"\n -------------- \n" +
                                $"Mensagem lida \n" +
                                $"Nome: {acao.Name} \n" +
                                $"Data: {acao.Date.ToString()} \n" +
                                $"Cotaçao: {acao.Cotacao} \n" +
                                $"Consume Group: {consumer.ConsumerGroup} \n" +
                                $"Particao: {partitionEvent.Partition.PartitionId} \n" +
                                $"Offset: {partitionEvent.Data.Offset} \n");
                    
                }
            }
            catch (TaskCanceledException)
            {
                // This is expected if the cancellation token is
                // signaled.
            }
            finally
            {
                await consumer.CloseAsync();
            }
        }
    }
}
