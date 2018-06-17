using System.Text;
using NUnit.Framework;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;
using SISPOSProxy.Core.Services;

namespace SISPOSProxy.Tests.ServicesTests
{
    [TestFixture]
    public class UdpRepairServiceTests
    {
        private readonly byte[] _inputWrongPANSPT;
        private readonly byte[] _inputWrongPANSSY;
        private readonly byte[] _inputOk;
        private readonly byte[] _inputNoFixingNeeded;

        private readonly DbCache _dbCache;

        public UdpRepairServiceTests()
        {
            _inputWrongPANSPT = Encoding.ASCII.GetBytes("$PANSPT,33,1,1*24\r\n$PANSPT,35,0,1*23\r\n$PANSSY,0,2,1,1,3,1,0*1A\r\n");
            _inputWrongPANSSY = Encoding.ASCII.GetBytes("$PANSPT,33,2,1*27\r\n$PANSPT,35,0,1*23\r\n$PANSSY,0,2,1,1,1,0,0*19\r\n");
            _inputOk = Encoding.ASCII.GetBytes("$PANSPT,33,2,1*27\r\n$PANSPT,35,0,1*23\r\n$PANSSY,0,2,1,1,3,1,0*1A\r\n");
            _inputNoFixingNeeded = Encoding.ASCII.GetBytes("$PANSSS,33,2,1*27\r\n$PANXXX,34,0,1*22\r\n$PANXXX,35,0,1*23\r\n");

            _dbCache = new DbCache();

            _dbCache.Save(new UdfPosMsg()
            {
                SectorId = 33,
                TagId = 20
            });
            _dbCache.Save(new UdfPosMsg()
            {
                SectorId = 33,
                TagId = 21
            });
            _dbCache.Save(new UdfPosMsg()
            {
                SectorId = 66,
                TagId = 45
            });
        }

        [Test]
        public void IsNeededToBeFixed()
        {
            var service = new UdpRepairService(_dbCache);

            var resultWrongPANSPT = service.IsNeededToBeFixed(_inputWrongPANSPT);
            var resultWrongPANSSY = service.IsNeededToBeFixed(_inputWrongPANSPT);
            var resultNo = service.IsNeededToBeFixed(_inputNoFixingNeeded);

            Assert.IsTrue(resultWrongPANSPT);
            Assert.IsTrue(resultWrongPANSSY);
            Assert.IsFalse(resultNo);
        }

        [Test]
        public void FixPANSPTModel()
        {
            var model = new UdpPanSpt()
            {
                SectorId = 33,
                TagsCount = 1,
                SectorStatus = SectorStatus.Mulfunction
            };
            var service = new UdpRepairService(_dbCache);

            service.FixPANSPT(model);

            Assert.AreEqual(33, model.SectorId);
            Assert.AreEqual(2, model.TagsCount);
            Assert.AreEqual(SectorStatus.Mulfunction, model.SectorStatus);
        }

        [Test]
        public void FixPANSSYModel()
        {
            var model = new UdpPanSsy()
            {
                SystemStatus = SystemStatus.NoDisturbance,
                Pos1Status = PosServerStatus.Initialization,
                Pos2Status = PosServerStatus.Disorder,
                SilenceStatus = SilenceStatus.Canceled,
                LoggedOnTagsAmount = 0,
                MissedTagsAmount = 0,
                IlasstStatus = IlasstStatus.NormalOperation
            };
            var service = new UdpRepairService(_dbCache);

            service.FixPANSSY(model);

            Assert.AreEqual(SystemStatus.NoDisturbance, model.SystemStatus);
            Assert.AreEqual(PosServerStatus.Initialization, model.Pos1Status);
            Assert.AreEqual(PosServerStatus.Disorder, model.Pos2Status);
            Assert.AreEqual(SilenceStatus.Canceled, model.SilenceStatus);
            Assert.AreEqual(3, model.LoggedOnTagsAmount);
            Assert.AreEqual(1, model.MissedTagsAmount);
            Assert.AreEqual(IlasstStatus.NormalOperation, model.IlasstStatus);
        }

        [Test]
        public void FixSentences_PANSPTWrong()
        {
            var service = new UdpRepairService(_dbCache);

            var payloadFixed = service.FixPayload(_inputWrongPANSPT);

            Assert.IsNotNull(payloadFixed);
            Assert.AreEqual(1, payloadFixed.Length);
            Assert.IsTrue(payloadFixed[0].ContainsSubArray(_inputOk));
        }

        [Test]
        public void FixSentences_PANSSYWrong()
        {
            var service = new UdpRepairService(_dbCache);

            var payloadFixed = service.FixPayload(_inputWrongPANSSY);

            Assert.IsNotNull(payloadFixed);
            Assert.AreEqual(1, payloadFixed.Length);
            Assert.IsTrue(payloadFixed[0].ContainsSubArray(_inputOk));
        }

        [Test]
        public void FixSentences_Ok()
        {
            var service = new UdpRepairService(_dbCache);

            var payload = service.FixPayload(_inputOk);

            Assert.IsNotNull(payload);
            Assert.AreEqual(1, payload.Length);
            Assert.IsTrue(payload[0].ContainsSubArray(_inputOk));
        }

        [Test]
        public void FixSentences_NoFixing()
        {
            var service = new UdpRepairService(_dbCache);

            var payload = service.FixPayload(_inputNoFixingNeeded);

            Assert.IsNotNull(payload);
            Assert.AreEqual(1, payload.Length);
            Assert.IsTrue(payload[0].ContainsSubArray(_inputNoFixingNeeded));
        }
    }
}
