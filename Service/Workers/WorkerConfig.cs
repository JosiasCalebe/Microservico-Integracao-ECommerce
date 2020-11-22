using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceIntegracao.Workers
{
    public class WorkerConfig
    {
        public string WorkerName { get; set; }
        public int Interval { get; set; }
        public bool Disabled { get; set; }
    }

    public class WorkerConfiguration
    {
        public List<WorkerConfig> WorkerConfigs { get; set; }
    }
}
