using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Core
{
    public class Proxy : IDisposable
    {
        private readonly CancellationTokenSource _cts;

        public Proxy()
        {
            _cts = new CancellationTokenSource();

            Settings.Init().Wait(_cts.Token);
        }

        public void Start()
        {

        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public void Dispose()
        {
            _cts.Dispose();
        }
    }
}
