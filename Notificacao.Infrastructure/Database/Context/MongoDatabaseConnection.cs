using Microsoft.Extensions.Configuration;
using Notificacao.Infrastructure.Config;

namespace Notificacao.ConsumerEmail.Context
{
    public class MongoDatabaseConnection
    {
        private readonly IConfiguration _configuration;

        public MongoDatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MongoDbContextConfig GetConnection()
        {
            var config = new MongoDbContextConfig();
            _configuration.Bind("MongoDB", config);
            return config;
        }
    }
}
