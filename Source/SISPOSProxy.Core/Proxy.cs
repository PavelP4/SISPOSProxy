using System;
using System.Threading;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Services;

namespace SISPOSProxy.Core
{
    public class Proxy : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;

        private DbReceiver _dbReceiver;
        private SISPOSReceiver _sisposReceiver;
        private SISPOSDataProcessor _sisposDataProcessor;
        private SISPOSTransmitter _sisposTransmitter;

        public Proxy()
        {
            _cts = new CancellationTokenSource();
            _token = _cts.Token;

            var settings = new Settings();
            settings.Init().Wait(_cts.Token);

            var dbCache = new DbCache();
            var inputCache = new MessageCache();
            var outputCache = new MessageCache();

            _dbReceiver = new DbReceiver(settings, dbCache);
            _sisposReceiver = new SISPOSReceiver(settings, inputCache);
            _sisposDataProcessor = new SISPOSDataProcessor(settings, dbCache, inputCache, outputCache);
            _sisposTransmitter = new SISPOSTransmitter(settings, outputCache);
        }

        public void Start()
        {
            _dbReceiver.Start();
            _sisposReceiver.Start();
            _sisposDataProcessor.Start();
            _sisposTransmitter.Start();
        }

        public void Stop()
        {
            _cts.Cancel(); 

            _sisposTransmitter.Stop();
            _sisposDataProcessor.Stop();
            _dbReceiver.Stop();
            _sisposReceiver.Stop();
        }

        public void Dispose()
        {
            if (!_token.IsCancellationRequested)
            {
                Stop();
            }

            _cts.Dispose();
            _dbReceiver.Dispose();
            _sisposReceiver.Dispose();
            _sisposDataProcessor.Dispose();
            _sisposTransmitter.Dispose();
        }
    }
}
