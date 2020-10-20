using System;
using System.Collections.Generic;
using System.Text;

namespace Notificacao.Infrastructure.Config
{
    public class MongoDbContextConfig
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
