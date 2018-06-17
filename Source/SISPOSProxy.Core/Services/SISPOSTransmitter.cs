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
        private const int MessageSendTimeout = 2;

        private readonly MessageCache _outputCache;

        public SISPOSTransmitter(Settings settings, MessageCache outputCache) 
            : base(settings)
        {
            _outputCache = outputCache;
        }

        public override void Start()
        {
            ServiceTasks.Add(Task.Factory.StartNew(RunSender, Token, TaskCreationOptions.LongRunning));
        }

        private void RunSender(object obj)
        {
            if (!Settings.TransmitIpEndPoints.Any()) throw new Exception("The Destination endpoints are not identified");

            using (var client = new UdpClient())
            {
                var tasks = new Task[Settings.TransmitIpEndPoints.Count];

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

                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = client.SendAsync(nextmsg, nextmsg.Length, Settings.TransmitIpEndPoints[i]);
                    }

                    Task.WaitAll(tasks, MessageSendTimeout, Token);
                }
            }
        }
    }
}
