using NUnit.Framework;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Tests.ModelsTests
{
    [TestFixture]
    public class UdfMessagesTests
    {
        [Test]
        public void TryParseUdfPosMsgModel()
        {
            var model = new UdfPosMsg()
            {
                TagId = 21,
                SectorId = 45
            };

            var parsed = UdfPosMsg.TryParse(model.ToBytes(), out var result);

            Assert.IsTrue(parsed, "UdfPosMsg was not parsed");
            Assert.IsNotNull(result, "result is null");
            Assert.AreEqual(model.TagId, result.TagId, "TagId is not equal");
            Assert.AreEqual(model.SectorId, result.SectorId, "SectorId is not equal");
        }

        [Test]
        public void TryParseUdfSecMsgModel()
        {
            var model = new UdfSecMsg()
            {
                SectorId = 45,
                SectorStatus = SectorStatus.Ok
            };

            var parsed = UdfSecMsg.TryParse(model.ToBytes(), out var result);

            Assert.IsTrue(parsed, "UdfSecMsg was not parsed");
            Assert.IsNotNull(result, "result is null");
            Assert.AreEqual(model.SectorId, result.SectorId, "SectorId is not equal");
            Assert.AreEqual(model.SectorStatus, result.SectorStatus, "SectorStatus is not equal");
        }

        [Test]
        public void TryParseUdfTagMsgModel()
        {
            var model = new UdfTagMsg()
            {
                TagId = 45,
                TagStatus = TagStatus.Ok
            };

            var parsed = UdfTagMsg.TryParse(model.ToBytes(), out var result);

            Assert.IsTrue(parsed, "UdfTagMsg was not parsed");
            Assert.IsNotNull(result, "result is null");
            Assert.AreEqual(model.TagId, result.TagId, "TagId is not equal");
            Assert.AreEqual(model.TagStatus, result.TagStatus, "TagStatus is not equal");
        }
    }
}
