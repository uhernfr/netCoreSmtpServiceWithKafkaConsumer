using System;
using System.Collections.Generic;
using System.Text;

namespace Notificacao.Domain.Interface
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
