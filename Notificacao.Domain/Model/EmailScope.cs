﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Notificacao.Domain
{
    public class EmailScope
    {
        public string Subject { get; set; }
        public string[] To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
        public string Instituto { get; set; }
    }
}
