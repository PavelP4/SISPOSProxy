using System;
using System.Text;
using NUnit.Framework;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Tests.ModelsTests
{
    [TestFixture]
    public class UdpSentencesTests
    {
        [Test]
        public void GetSentenceType()
        {
            var panspt = new ArraySegment<byte>(Encoding.ASCII.GetBytes("$PANSPT,33,15,1*11\r\n"));
            var panssy = new ArraySegment<byte>(Encoding.ASCII.GetBytes("$PANSSY,0,2,1,1,10,2,0*2B\r\n"));
            var panWrong = new ArraySegment<byte>(Encoding.ASCII.GetBytes("$PANXXX,0*2B\r\n"));

            Assert.AreEqual(UdpSentenceType.PANSPT, UdpSentence.GetSentenceType(panspt), "PANSPT is not recognized");
            Assert.AreEqual(UdpSentenceType.PANSSY, UdpSentence.GetSentenceType(panssy), "PANSSY is not recognized");
            Assert.AreEqual(UdpSentenceType.Undefined, UdpSentence.GetSentenceType(panWrong), "Wrong is not recognized");
        }

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
            var inputStr = Encoding.ASCII.GetString(model.ToBytes());
            var inputStrOrigin = "$PANSPT,33,15,1*11\r\n";

            var parsed = UdpPanSpt.TryParse(input, out var result);

            Assert.IsTrue(parsed, "UdpPanSpt was not parsed");
            Assert.IsNotNull(result, "result is null");
            Assert.AreEqual(model.SectorId, result.SectorId, "SectorId is not equal");
            Assert.AreEqual(model.TagsCount, result.TagsCount, "TagsCount is not equal");
            Assert.AreEqual(model.SectorStatus, result.SectorStatus, "TagsCount is not equal");

            Assert.AreEqual(inputStrOrigin, inputStr, "Origin is not equal to str from model");
        }

        [Test]
        public void TryParseUdpPanSsySentence()
        {
            var model = new UdpPanSsy()
            {
                SystemStatus = SystemStatus.NoDisturbance,
                Pos1Status = PosServerStatus.Initialization,
                Pos2Status = PosServerStatus.Disorder,
                SilenceStatus = SilenceStatus.Canceled,
                LoggedOnTagsAmount = 10,
                MissedTagsAmount = 2,
                IlasstStatus = IlasstStatus.NormalOperation
            };
            var input = new ArraySegment<byte>(model.ToBytes());
            var inputStr = Encoding.ASCII.GetString(model.ToBytes());
            var inputStrOrigin = "$PANSSY,0,2,1,1,10,2,0*2B\r\n";

            var parsed = UdpPanSsy.TryParse(input, out var result);

            Assert.IsTrue(parsed, "UdpPanSsy was not parsed");
            Assert.IsNotNull(result, "result is null");
            Assert.AreEqual(model.SystemStatus, result.SystemStatus, "SystemStatus is not equal");
            Assert.AreEqual(model.Pos1Status, result.Pos1Status, "Pos1Status is not equal");
            Assert.AreEqual(model.Pos2Status, result.Pos2Status, "Pos2Status is not equal");
            Assert.AreEqual(model.SilenceStatus, result.SilenceStatus, "SilenceStatus is not equal");
            Assert.AreEqual(model.LoggedOnTagsAmount, result.LoggedOnTagsAmount, "LoggedOnTagsAmount is not equal");
            Assert.AreEqual(model.MissedTagsAmount, result.MissedTagsAmount, "MissedTagsAmount is not equal");
            Assert.AreEqual(model.IlasstStatus, result.IlasstStatus, "IlasstStatus is not equal");

            Assert.AreEqual(inputStrOrigin, inputStr, "Origin is not equal to str from model");
        }
    }
}
