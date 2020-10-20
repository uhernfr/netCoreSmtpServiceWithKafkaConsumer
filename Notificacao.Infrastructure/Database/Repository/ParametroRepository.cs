using MongoDB.Driver;
using Notificacao.Domain.Interface;
using Notificacao.Domain.Model;
using System.Threading.Tasks;

namespace Notificacao.Infrastructure.Repository
{
    public class ParametroRepository : RepositoryMongo<Parametro>, IParametroRepository
    {        
        protected readonly IMongoCollection<Parametro> Collectionparam;
        private readonly IMongoDatabase _db;
        public ParametroRepository(IMongoClient client, IMongoDatabase db) : base(client, db)
        {            
            Collectionparam = db.GetCollection<Parametro>("AdministrativoParametro");                       
            _db = db;
        }

        public async Task<Parametro> GetParametroAsync(string tipo)
        {
            var paramDB = Collectionparam.Find(x => x.Tipo == tipo);

            return paramDB.FirstOrDefault();
        }

    }
}
