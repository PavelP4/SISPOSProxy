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
            ServiceTasks.Add(Task.Factory.StartNew(ReceivePackets, Token, TaskCreationOptions.LongRunning));
        }
        
        private void ReceivePackets(object obj)
        {
            if (!Settings.ListenPort.HasValue) throw new Exception("The listen port is not identified");

            using (var client = new UdpClient(Settings.ListenPort.Value))
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
