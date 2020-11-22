using ServiceIntegracao.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Linq;

namespace ServiceIntegracao.Workers
{
    public abstract class WorkerBase<T> : IHostedService, IDisposable
    {
        private Mutex mutex;
        protected System.Timers.Timer _timer;
        protected delegate void ElapsedTimeDelegate(object sender, ElapsedEventArgs e);
        protected ElapsedTimeDelegate ElapsedTime;
        private int _interval = 0;
        private readonly IServiceProvider serviceProvider;
        private bool firstExecution = true;
        private int? customInterval = null;
        private bool disabled = false;

        public WorkerBase(ILogger<T> logger, IServiceProvider serviceProvider, string mutexName, int? customInterval = null)
        {
            mutex = new Mutex(false, mutexName);
            this.serviceProvider = serviceProvider;
            this.customInterval = customInterval;
            if (customInterval == null)
            {
                _interval = GetInterval();
            }
            else
            {
                _interval = Convert.ToInt32(customInterval);
            }

            CreateTimer();
        }

        public WorkerBase(ILogger<T> logger, IServiceProvider serviceProvider, string mutexName, WorkerConfiguration workerConfiguration)
        {
            mutex = new Mutex(false, mutexName);
            this.serviceProvider = serviceProvider;
            
            
            this.customInterval = workerConfiguration?.WorkerConfigs?.FirstOrDefault(x => x.WorkerName == mutexName)?.Interval; 
            if (customInterval == null)
            {
                _interval = GetInterval();
            }
            else
            {
                _interval = Convert.ToInt32(customInterval);
            }
            var disabled = workerConfiguration?.WorkerConfigs?.FirstOrDefault(x => x.WorkerName == mutexName)?.Disabled;
            if (disabled != null)
            {
                this.disabled = (bool)disabled;
            }

            CreateTimer();
        }

        private int GetInterval()
        {
            var interval = 0;
            if (customInterval == null)
            {
                interval = serviceProvider.GetService<IOptionsMonitor<AppSettings>>().CurrentValue.ServiceConfiguration.TimeInterval;
            }
            else
            {
                interval = _interval;
            }
            return interval;
        }

        private void CreateTimer()
        {
            var interval = 1000;
            _timer = new System.Timers.Timer(interval);
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;            
        }

        private int CreateInterval()
        {
            return _interval * 60 * 1000;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!disabled)
            {
                if (ElapsedTime != null)
                {
                    if (mutex.WaitOne(500))
                    {
                        try
                        {
                            var checkInterval = GetInterval();
                            if (_interval != checkInterval || firstExecution)
                            {
                                firstExecution = false;
                                _interval = checkInterval;
                                _timer.Interval = CreateInterval();
                            }

                            var task = Task.Run(() => ElapsedTime(sender, e));

                            task.Wait();
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine($"Erro {ex.Message}");
                        }

                        mutex.ReleaseMutex();
                    }
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _timer.Enabled = true;
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _timer.Enabled = false;
            });
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
