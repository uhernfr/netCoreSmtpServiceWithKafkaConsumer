using MongoDB.Driver.Linq;
using Notificacao.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Notificacao.Domain.Interface
{
    public interface IRepository<TEntity> where TEntity :Entity
    {
        
        IMongoQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();

        void Registe(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
