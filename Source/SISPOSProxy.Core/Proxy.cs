using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Receivers;

namespace SISPOSProxy.Core
{
    public class Proxy : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;

        private readonly Settings _settings;
        private readonly DbCache _dbCache;
        private readonly DbReceiver _dbReceiver;

        public Proxy()
        {
            _cts = new CancellationTokenSource();
            _token = _cts.Token;

            _settings = new Settings();
            _settings.Init().Wait(_token);

            _dbCache = new DbCache();

            _dbReceiver = new DbReceiver(_settings, _dbCache);
        }

        public void Start()
        {
            _dbReceiver.Start();
        }

        public void Stop()
        {
            _cts.Cancel();

            _dbReceiver.Stop();
        }

        public void Dispose()
        {
            _cts.Dispose();
            _dbReceiver.Dispose();
        }
    }
}
