using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
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
            _settings.Init().Wait();
        }

        [Test]
        public void ReceivePackets()
        {
            var msgCache = new MessageCache();
            var sender = new UdpPacketSender();

            var msg = Encoding.ASCII.GetBytes("$PANSPT,33,22,1*88\r\n$PANSPT,44,55,1*99\r\n");

            using (var receiver = new SISPOSReceiver(_settings, msgCache))
            {
                receiver.Start();
               
                sender.Send(_settings.ListenIpEndPoint, msg);
            }

            Assert.AreEqual(1, msgCache.Count, "The message was not received");
        }
    }
}
