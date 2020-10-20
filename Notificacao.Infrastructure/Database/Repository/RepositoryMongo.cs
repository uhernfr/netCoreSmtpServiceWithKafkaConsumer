using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Notificacao.Domain.Interface;
using Notificacao.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Notificacao.Infrastructure.Repository
{
    public class RepositoryMongo<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;

        protected readonly IMongoCollection<TEntity> Collection;

        public RepositoryMongo(IMongoClient client, IMongoDatabase db, string collection = "AdministrativoParametro")
        {
            _client = client;
            _db = db;
            Collection = db.GetCollection<TEntity>(collection);
        }

        public IMongoQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate) =>
            Collection.AsQueryable().Where(predicate);

        public IEnumerable<TEntity> GetAll() =>
            Collection.Find(_ => true).ToList();

        public void Registe(TEntity entity)
        {
            Collection.InsertOneAsync(entity);
        }
        public void Update(TEntity entity)
        {
            Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
        }

        public void Delete(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            Collection.DeleteOneAsync(filter);
        }
    }
}
