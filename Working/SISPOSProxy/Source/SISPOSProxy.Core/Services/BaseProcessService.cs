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

            ServiceTasks.Clear();
        }

        public virtual void Dispose()
        {
            if (_cts != null && !Token.IsCancellationRequested)
            {
                Stop();
            }

            _cts?.Dispose();
        }
    }
}
