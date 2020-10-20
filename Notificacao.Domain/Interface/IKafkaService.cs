using System;
using System.Collections.Generic;
using System.Text;

namespace Notificacao.Domain.Interface
{
    public interface IKafkaService
    {
        bool CheckKafkaAsync();

        string SendMessageByKafka(string message, string fila, string bootstrapServers);
    }
}
