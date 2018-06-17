using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Helpers;
using SISPOSProxy.Core.Services;
using SISPOSProxy.Tests.Misc;

namespace SISPOSProxy.Tests.ServicesTests
{
    [TestFixture]
    public class SISPOSTransmitterTests
    {
        private readonly Settings _settings = new Settings();
        private readonly int[] _ports = {55004, 55005 };

        public SISPOSTransmitterTests()
        {
            var localIpAddress = NetHelper.GetLocalIPv4(NetworkInterfaceType.Ethernet);

            _settings.TransmitIpEndPoints = new List<IPEndPoint>();

            foreach (var port in _ports)
            {
                _settings.TransmitIpEndPoints.Add(new IPEndPoint(localIpAddress, port));
            }
        }

        [Test]
        public void SendingTest()
        {
            var messageCache = new MessageCache();
            var testReceiver = new UdpPacketReceiver();
            var payloads = new[]
            {
                Encoding.ASCII.GetBytes("$PANSPT,33,22,1*88\r\n$PANSPT,44,55,1*99\r\n"),
                Encoding.ASCII.GetBytes("$PANSPT,44,55,1*77\r\n$PANSPT,11,22,1*55\r\n")
            };
            var result = new byte[_ports.Length][];

            foreach (var payload in payloads)
            {
                messageCache.Push(payload);
            }

            using (var transmitter = new SISPOSTransmitter(_settings, messageCache))
            {
                var tasks = new Task<byte[]>[_ports.Length];

                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = testReceiver.ReceiveAsync(_ports[i]);
                }
                
                Thread.Sleep(100);

                transmitter.Start();

                Task.WaitAll(tasks, 1000);

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = tasks[i].Result;
                }
            }

            Assert.IsTrue(result.All(x => x != null && x.Length > 0));
            Assert.IsTrue(result.All(x => payloads[0].ContainsSubArray(x)));
        }
    }
}
