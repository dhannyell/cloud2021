using Confluent.Kafka;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace KafkaProducer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            logger.Information("Envio de Mensagens para o Broker Kafka");

            var bootstrapServer = ConfigurationManager.AppSettings.Get("bootstrapServers");
            var topicName = ConfigurationManager.AppSettings.Get("topicName");
            var password = ConfigurationManager.AppSettings.Get("password");

            logger.Information($"BootstrapServers: {bootstrapServer}");
            logger.Information($"Topic Name: {topicName}");


            logger.Information("Digite a Mensagem que será enviada");
            var mensagem = Console.ReadLine();

            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServer,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = "$ConnectionString",
                    SaslPassword = password
                };

                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    var result = await producer.ProduceAsync(
                        topicName,
                        new () { Value = mensagem}
                    );

                    logger.Information($"Mensagem: {mensagem} | " +
                        $"Status: {result.Status.ToString()}");
                }
            }
            catch (Exception e)
            {
                logger.Error($"Exception: {e.GetType().FullName} | " +
                    $"Mensagem: {e.Message}");
            }

        }
    }
}
