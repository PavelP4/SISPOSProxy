using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
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
    public class SISPOSReceiverTests
    {
        private readonly Settings _settings = new Settings();

        public SISPOSReceiverTests()
        {
            _settings.ListenPort = 55554;
        }

        [Test]
        public void ReceivePackets()
        {
            if (!_settings.ListenPort.HasValue) throw new ArgumentException("Listen port is not initialized");

            var msgCache = new MessageCache();
            var sender = new UdpPacketSender();
            var packetsCount = 10;
          
            var msg = Encoding.ASCII.GetBytes("$PANSPT,33,22,1*88\r\n$PANSPT,44,55,1*99\r\n");

            using (var receiver = new SISPOSReceiver(_settings, msgCache))
            {
                receiver.Start();

                for (int i = 0; i < packetsCount; i++)
                {
                   sender.SendToLocalhost(_settings.ListenPort.Value, msg); 
                }
            }

            Assert.AreEqual(packetsCount, msgCache.Count, "The message was not received");
            Assert.IsTrue(msgCache.Pop().ContainsSubArray(msg));
        }
    }
}
