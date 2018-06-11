using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Core.Services
{
    public class SISPOSTransmitter : BaseProcessService
    {
        private const int MessageSenderSleepTimeout = 1;

        private readonly MessageCache _outputCache;

        public SISPOSTransmitter(Settings settings, MessageCache outputCache) 
            : base(settings)
        {
            _outputCache = outputCache;
        }

        public override void Start()
        {
            if (!Settings.TransmitIpEndPoints.Any()) throw new Exception("The Destination endpoints are not identified");

            foreach (var iepoint in Settings.TransmitIpEndPoints)
            {
                ServiceTasks.Add(Task.Factory.StartNew((o)=> RunSender(iepoint), Token, TaskCreationOptions.LongRunning));
            }
        }

        private void RunSender(IPEndPoint iepoint)
        {
            using (var client = new UdpClient(iepoint))
            {
                while (!Token.IsCancellationRequested)
                {
                    var nextmsg = _outputCache.Pop();

                    if (nextmsg.Length == 0)
                    {
                        if (_outputCache.Count == 0)
                        {
                            Thread.Sleep(MessageSenderSleepTimeout);
                        }
                        
                        continue;
                    }

                    client.Send(nextmsg, nextmsg.Length);
                }
            }
        }
    }
}
