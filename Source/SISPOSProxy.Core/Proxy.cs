using System;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Services;

namespace SISPOSProxy.Core
{
    public class Proxy : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;

        private readonly DbReceiver _dbReceiver;
        private readonly SISPOSReceiver _sisposReceiver;
        private readonly SISPOSDataProcessor _sisposDataProcessor;
        private readonly SISPOSTransmitter _sisposTransmitter;

        public Proxy()
        {
            _cts = new CancellationTokenSource();
            _token = _cts.Token;

            var settings = new Settings();
            var dbCache = new DbCache();
            var inputCache = new MessageCache();
            var outputCache = new MessageCache();

            var settingsInitTask = settings.InitAsync();
            var dbCacheInitTask = dbCache.InitAsync();
            Task.WaitAll(new[]{settingsInitTask, dbCacheInitTask}, _token);

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
