using Confluent.Kafka;
using KafkaConsumer.Models;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Configuration;
using System.Threading;

namespace KafkaConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            logger.Information("Consumer Broker Kafka");

            var bootstrapServer = ConfigurationManager.AppSettings.Get("bootstrapServers");
            var topicName = ConfigurationManager.AppSettings.Get("topicName");
            var password = ConfigurationManager.AppSettings.Get("password");

            logger.Information($"BootstrapServers: {bootstrapServer}");
            logger.Information($"Topic Name: {topicName}");

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServer,
                GroupId = $"grupo1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = password
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                using (var consumer = new ConsumerBuilder<string, string>(config).Build())
                {
                    consumer.Subscribe(topicName);

                    try
                    {
                        while (true)
                        {
                            var cr = consumer.Consume(cts.Token);

                            Acao acao = JsonConvert.DeserializeObject<Acao>(cr.Message.Value);

                            logger.Information(
                                $"\n\n -------------- \n"+
                                $"Mensagem lida \n" +
                                $"Nome: {acao.Name} \n" +
                                $"Data: {acao.Date.ToString()} \n" +
                                $"Cotaçao: {acao.Cotacao} \n" + 
                                $"Consume Group: {config.GroupId} \n" +
                                $"Particao: {cr.Partition} \n" + 
                                $"Offset: {cr.Offset} \n");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumer.Close();
                        logger.Warning("Cancelada a execução do Consumer...");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Exceção: {ex.GetType().FullName} | " +
                             $"Mensagem: {ex.Message}");
            }
        }
    }
}
