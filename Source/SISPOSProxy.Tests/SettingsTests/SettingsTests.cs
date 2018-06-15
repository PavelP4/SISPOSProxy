using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Tests.SettingsTests
{
    [TestFixture]
    public class SettingsTests
    {
        public const string LocalIPAddress = "192.168.10.133";
        public const int ListenPort = 55554;

        public const string SendToIPAddress = "192.168.10.100";
        public const int SendToPort = 55554;

        [Test]
        public void InitTest()
        {
            var settings = new Settings();

            settings.Init().Wait();

            Assert.IsTrue(!string.IsNullOrEmpty(settings.Udf2ProxyNamedPipeName));
            Assert.IsTrue(settings.Udf2ProxyNamedPipeMaxServerInstances > 1);

            Assert.IsNotNull(settings.ListenIpEndPoint);
            Assert.AreEqual(LocalIPAddress, settings.ListenIpEndPoint.Address.ToString());
            Assert.AreEqual(ListenPort, settings.ListenIpEndPoint.Port);

            Assert.IsNotNull(settings.TransmitIpEndPoints);
            Assert.AreEqual(2, settings.TransmitIpEndPoints);
            Assert.IsTrue(settings.TransmitIpEndPoints.Any(x => x.Address.ToString() == SendToIPAddress && x.Port == SendToPort));
        }
    }
}
