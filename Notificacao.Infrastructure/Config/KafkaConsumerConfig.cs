using System;
using System.Collections.Generic;
using System.Text;

namespace Notificacao.Infrastructure.Config
{
    public class KafkaConsumerConfig
    {
        public string BootstrapServers { get; set; }
        public string AutoOffsetReset { get; set; }
        public string EnableAutoCommit { get; set; }
        public string SecurityProtocol { get; set; }
        public string SaslMechanism { get; set; }
        public string SaslUsername { get; set; }
        public string SaslPassword { get; set; }
        public string ClientId { get; set; }
        public string GroupId { get; set; }
        public string Topic { get; set; }
    }
}
