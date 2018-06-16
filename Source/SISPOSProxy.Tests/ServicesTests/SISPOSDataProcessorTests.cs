using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;
using SISPOSProxy.Core.Services;

namespace SISPOSProxy.Tests.ServicesTests
{
    [TestFixture]
    public class SISPOSDataProcessorTests
    {
        private readonly Settings _settings = new Settings();

        public SISPOSDataProcessorTests()
        {
            //_settings
        }

        [Test]
        public void TagsCountFix()
        {
            var dbCache = new DbCache();
            var inputCache = new MessageCache();
            var outputCache = new MessageCache();

            dbCache.Save(new UdfPosMsg()
            {
                SectorId = 33,
                TagId = 20
            });
            dbCache.Save(new UdfPosMsg()
            {
                SectorId = 33,
                TagId = 21
            });

            var payloadWrong = Encoding.ASCII.GetBytes("$PANSPT,33,1,1*24\r\n$PANSPT,34,0,1*22\r\n$PANSPT,35,0,1*23\r\n");
            var payloadOk = Encoding.ASCII.GetBytes("$PANSPT,33,2,1*27\r\n$PANSPT,34,0,1*22\r\n$PANSPT,35,0,1*23\r\n");

            using (var service = new SISPOSDataProcessor(_settings, dbCache, inputCache, outputCache))
            {
                service.Start();

                inputCache.Push(payloadWrong);
                
                Thread.Sleep(100);
            }

            var result = outputCache.Pop();
            var resultStr = Encoding.ASCII.GetString(result);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 1);
            Assert.IsTrue(result.ContainsSubArray(payloadOk), "Repared payload is not correct");
        }
    }
}
