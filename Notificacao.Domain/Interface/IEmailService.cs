using System;
using System.Collections.Generic;
using System.Text;

namespace Notificacao.Domain.Interface
{
    public interface IEmailService
    {
        void EnviarEmail(EmailScope request);
    }
}
