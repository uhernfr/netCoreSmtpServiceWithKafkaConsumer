using System;
using System.Collections.Generic;
using System.Text;

namespace Notificacao.Domain.Model
{
    public class ParametroEmailDto 
    {
        public string Host { get; set; }
        public string Porta { get; set; }
        public string EmailRemetente { get; set; }
        public string NomeRemetente { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string SSL { get; set; }
    }
}
