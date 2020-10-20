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
using System.Threading.Tasks;

namespace Notificacao.ConsumerEmail
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IParametroRepository _parametroRepository;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmailService _emailService;
        private readonly IKafkaService _kafkaService;
        public Worker(ILogger<Worker> logger, IParametroRepository parametroRepository,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            IEmailService emailService,
            IKafkaService kafkaService)
        {
            _logger = logger;
            _parametroRepository = parametroRepository;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _emailService = emailService;
            _kafkaService = kafkaService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_kafkaService.CheckKafkaAsync())
            {
                while (!cancellationToken.IsCancellationRequested)
                {                    
                    await Task.Delay(1000, cancellationToken);
                    var configKafka = new KafkaConsumerConfig();
                    _configuration.Bind("Kafka", configKafka);

                    var conf = new ConsumerConfig
                    {
                        //GroupId = "test-consumer-group",
                        //BootstrapServers = "localhost:9092"
                        GroupId = configKafka.GroupId ?? "unnamed",
                        BootstrapServers = configKafka.BootstrapServers, // "192.168.1.100:9092",
                        AutoOffsetReset = AutoOffsetReset.Earliest
                    };

                    using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
                    {
                        c.Subscribe(configKafka.Topic);

                        try
                        {
                            //timeout 
                            TimeSpan timeou = new TimeSpan(0, 0, 5);
                            // var message = c.Consume(cancellationToken);
                            var message = c.Consume(cancellationToken);
                            var email = JsonConvert.DeserializeObject<EmailScope>(message.Value);

                            _logger.LogDebug($"--Mensagem Recebida \n  Topico....: {configKafka.Topic} \n  Conteudo..: {JsonConvert.SerializeObject(message.Value, Formatting.Indented)}");

                            _emailService.EnviarEmail(email);
                        }
                        catch (Exception ex)
                        {
                            var message = c.Consume(cancellationToken);
                            _logger.LogDebug($"--Erro Kafka Consumer: \n  Topico....: {configKafka.Topic} \n  Conteudo..: {JsonConvert.SerializeObject(message.Value, Formatting.Indented)}\n  Mensagem..: {ex.Message}");

                        }
                        finally
                        {
                            c.Close();
                        }
                    }
                }
            }
        }
    }
}
