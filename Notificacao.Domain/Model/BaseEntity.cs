using Notificacao.Domain.Interface;
using System;

namespace Notificacao.Domain.Model
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; }
        public DateTime DataCriacao { get; private set; } = DateTime.Now;
    }
}
