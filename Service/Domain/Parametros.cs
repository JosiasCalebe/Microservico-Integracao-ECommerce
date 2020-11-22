using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceIntegracao.Domain
{
    public class RetornoLog
    {
        public string acao { get; set; }
        public string message { get; set; }
        public object request { get; set; }
        public object response { get; set; }
        public DateTime dataAlteracao { get { return DateTime.Now; } }
    }
}
