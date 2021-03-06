﻿using NUnit.Framework;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Tests.SettingsTests
{
    [TestFixture]
    public class SettingsTests
    {
        [Test]
        public void InitTest()
        {
            var settings = new Settings();

            settings.InitAsync().Wait();

            Assert.IsTrue(!string.IsNullOrEmpty(settings.Udf2ProxyNamedPipeName));
            Assert.IsTrue(settings.Udf2ProxyNamedPipeMaxServerInstances > 1);

            Assert.IsNotNull(settings.ListenPort);
            Assert.IsTrue(settings.ListenPort.HasValue && settings.ListenPort.Value > 0);

            Assert.IsNotNull(settings.TransmitIpEndPoints);
            Assert.IsTrue(settings.TransmitIpEndPoints.Count >= 1);
        }
    }
}
