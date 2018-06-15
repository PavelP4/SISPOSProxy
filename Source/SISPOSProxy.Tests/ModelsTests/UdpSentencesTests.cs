using System;
using NUnit.Framework;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Tests.ModelsTests
{
    [TestFixture]
    public class UdpSentencesTests
    {
        [Test]
        public void TryParseUdpPanSptSentence()
        {
            var model = new UdpPanSpt()
            {
                SectorId = 33,
                TagsCount = 15,
                SectorStatus = SectorStatus.Ok
            };
            var input = new ArraySegment<byte>(model.ToBytes());

            var parsed = UdpPanSpt.TryParse(input, out var result);

            Assert.IsTrue(parsed, "UdpPanSpt was not parsed");
            Assert.IsNotNull(result, "result is null");
            Assert.AreEqual(model.SectorId, result.SectorId, "SectorId is not equal");
            Assert.AreEqual(model.TagsCount, result.TagsCount, "TagsCount is not equal");
            Assert.AreEqual(model.SectorStatus, result.SectorStatus, "TagsCount is not equal");
        }
    }
}
