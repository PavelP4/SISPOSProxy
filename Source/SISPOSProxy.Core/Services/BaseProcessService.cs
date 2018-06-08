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

        public BaseProcessService(Settings settings)
        {
            Settings = settings;           

            _cts = new CancellationTokenSource();
            Token = _cts.Token;
        }

        public abstract void Start();

        public virtual void Stop()
        {
            _cts.Cancel();
        }

        public virtual void Dispose()
        {
            _cts?.Dispose();
        }
    }
}
