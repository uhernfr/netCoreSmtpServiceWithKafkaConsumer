using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Notificacao.Domain;
using Notificacao.Domain.Interface;
using Notificacao.Domain.Model;
using Notificacao.Infrastructure.Config;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;

namespace Notificacao.ConsumerEmail
{
    public class KafkaService : IKafkaService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IParametroRepository _parametroRepository;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public KafkaService(ILogger<EmailService> logger, IParametroRepository parametroRepository,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _parametroRepository = parametroRepository;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public bool CheckKafkaAsync()
        {
            var result = new StringBuilder();
            var ret = true;

            try
            {
                // var result = new StringBuilder();
                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                var topics = new string[] { "fila_health_check" };
                var config = new KafkaConsumerConfig();

                _configuration.Bind("Kafka", config);
                if (string.IsNullOrEmpty(config.BootstrapServers))
                    result.AppendLine("--Kafka Servers connection is empty.");

                result.AppendLine("--BootstrapServers...: " + config.BootstrapServers);
                result.AppendLine("--GroupId............: " + config.GroupId);

                _logger.LogDebug($"{result.ToString()}");
                result.Clear();

                //teste producer
                SendMessageByKafka("fila_health_check: Notificacao.Consumer.Email - gravacao e leitura na fila Kafka OK", "fila_health_check", config.BootstrapServers);

                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = config.BootstrapServers,
                    GroupId = config.GroupId,
                    //StatisticsIntervalMs = 300,
                    //SessionTimeoutMs = 300
                    //AutoOffsetReset = AutoOffsetReset.Earliest,
                    //EnablePartitionEof = true
                    // partition offsets can be committed to a group even by consumers not
                    // subscribed to the group. in this example, auto commit is disabled
                    // to prevent this from occurring.
                    EnableAutoCommit = true
                };
                //const int commitPeriod = 5;
                using (var consumer =
                               new ConsumerBuilder<Ignore, string>(consumerConfig)
                                   .SetErrorHandler((_, e) => result.AppendLine($"--Error: {e.Reason}"))
                                   .Build())
                {
                    consumer.Assign(topics.Select(topic => new TopicPartitionOffset("fila_health_check", 0, Offset.Beginning)).ToList());
                    try
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(new TimeSpan(0, 0, 10));
                            result.AppendLine($"--Mensagem Recebida..:\n  {consumeResult?.TopicPartitionOffset}: ${consumeResult?.Value}");
                        }
                        catch (ConsumeException e)
                        {
                            result.AppendLine($"--Consumer error: {e.Error.Reason}");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        result.AppendLine("--Closing consumer.");
                        consumer.Close();
                    }
                }

                _logger.LogDebug($"{result.ToString()}");
            }
            catch (Exception ex)
            {
                //var result = new StringBuilder();
                result.AppendLine("--Exception EX." + ex.Message);
                _logger.LogDebug($"{result.ToString()}");
                ret = false;
            }
            return ret;
        }
        public string SendMessageByKafka(string message, string fila, string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var sendResult = producer
                                        .ProduceAsync(fila, new Message<Null, string> { Value = message })
                                            .GetAwaiter()
                                                .GetResult();

                    _logger.LogDebug($"--Mensagem Gravada...:\n  '{sendResult.Value}' de '{sendResult.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    //Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    _logger.LogDebug($"--Erro Kafka Producer:\n  {e.Error.Reason}");
                }
            }

            return string.Empty;
        }

    }
}
