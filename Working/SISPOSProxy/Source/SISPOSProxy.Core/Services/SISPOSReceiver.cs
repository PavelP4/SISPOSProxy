using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Extentions;

namespace SISPOSProxy.Core.Services
{
    public class SISPOSReceiver : BaseProcessService
    {
        private readonly MessageCache _inputCache;

        private readonly ManualResetEvent _cancelUdpClient;
        private readonly CancellationTokenRegistration _regOfCancelUdpClient;

        public SISPOSReceiver(Settings settings, MessageCache inputCache)
            :base(settings)
        {
            _inputCache = inputCache;

            _cancelUdpClient = new ManualResetEvent(false);
            _regOfCancelUdpClient = Token.Register(() => _cancelUdpClient.Set());
        }

        public override void Start()
        {
            _cancelUdpClient.Reset();

            ServiceTasks.Add(Task.Factory.StartNew(ReceivePackets, Token, TaskCreationOptions.LongRunning));
        }
        
        private void ReceivePackets(object obj)
        {
            if (!Settings.ListenPort.HasValue) throw new Exception("The listen port is not identified");

            using (var client = new UdpClient(Settings.ListenPort.Value))
            {
                while (!Token.IsCancellationRequested)
                {
                    byte[] msg = client.ReceiveEx(_cancelUdpClient);
                    _inputCache.Push(msg);
                }
            }
        }

        public override void Dispose()
        {
            _regOfCancelUdpClient.Dispose();
            _cancelUdpClient.Dispose();

            base.Dispose();
        }
    }
}
