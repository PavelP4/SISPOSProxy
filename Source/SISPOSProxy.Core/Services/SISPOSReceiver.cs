using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Core.Services
{
    public class SISPOSReceiver : BaseProcessService
    {
        private readonly MessageCache _inputCache;
        
        public SISPOSReceiver(Settings settings, MessageCache inputCache)
            :base(settings)
        {
            _inputCache = inputCache;
        }

        public override void Start()
        {
            if (Settings.ListenIpEndPoint == null) throw new Exception("Listen port is not identified");

            Task.Factory.StartNew(ReceivePackets, Token, TaskCreationOptions.LongRunning);
        }
        
        private void ReceivePackets(object obj)
        {
            using (var client = new UdpClient(Settings.ListenIpEndPoint))
            {
                IPEndPoint remote = null;

                while (!Token.IsCancellationRequested)
                {
                    byte[] msg = client.Receive(ref remote);

                    _inputCache.Push(msg);
                }
            }
        }
    }
}
