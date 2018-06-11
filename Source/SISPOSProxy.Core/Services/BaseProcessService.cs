using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Core.Services
{
    public abstract class BaseProcessService : IDisposable
    {
        private readonly TimeSpan _tasksFinishingWaitTimeout = TimeSpan.FromSeconds(2);

        private readonly CancellationTokenSource _cts;

        protected readonly CancellationToken Token;
        protected readonly Settings Settings;
        protected readonly List<Task> ServiceTasks;

        public BaseProcessService(Settings settings)
        {
            Settings = settings;           

            _cts = new CancellationTokenSource();
            Token = _cts.Token;

            ServiceTasks = new List<Task>();
        }

        public abstract void Start();

        public virtual void Stop()
        {
            _cts.Cancel();

            Task.WaitAll(ServiceTasks.ToArray(), _tasksFinishingWaitTimeout); 
        }

        public virtual void Dispose()
        {
            _cts?.Dispose();
        }
    }
}
