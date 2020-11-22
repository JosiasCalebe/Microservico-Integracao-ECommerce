using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceIntegracao.Settings
{
    public class AppSettings
    {
        public string Plataforma { get; set; }
        public ServiceConfiguration ServiceConfiguration { get; set; }
    }

    public class ServiceConfiguration
    {
        public int TimeInterval { get; set; }
    }
}
