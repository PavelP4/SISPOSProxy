﻿using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SISPOSProxy.Core;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Models;
using SISPOSProxy.Core.Receivers;
using SISPOSProxy.Tests.Senders;

namespace SISPOSProxy.Tests
{
    [TestFixture]
    public class DbReceiverTest
    {
        private readonly Settings _settings = new Settings();

        public DbReceiverTest()
        {
            _settings.Init().Wait();
        }
        
        [Test]
        public void CanReceiveAllMessageTypes()
        {
            var dbcache = new DbCache();
            var tagMsg = new UdfTagMsgModel
            {
                TagId = 15,
                TagStatus = TagStatus.Inactive
            };
            var posMsg = new UdfPosMsgModel
            {
                TagId = 15,
                SectorId = 44
            };
            var secMsg = new UdfSecMsgModel
            {
                SectorId = 44,
                SectorStatus = SectorStatus.Ok
            };

            using (var receiver = new DbReceiver(_settings, dbcache))
            using (var sender = new UdfMessageSender(_settings))
            {
                receiver.Start();

                sender.Send(tagMsg);
                sender.Send(posMsg);
                sender.Send(secMsg);

                Thread.Sleep(100);

                Assert.AreEqual(tagMsg.TagStatus, dbcache.GetTagStatus(tagMsg.TagId), "Tag status was not received");
                Assert.AreEqual(posMsg.SectorId, dbcache.GetTagSector(posMsg.TagId), "Tag's sector is not correct");
                Assert.AreEqual(secMsg.SectorStatus, dbcache.GetSectorStatus(secMsg.SectorId), "Sector status was not received");
            }
        }

        [Test]
        public void CanReceiveHugeAmountOfMessages()
        {
            var dbcache = new DbCache();
            var tagMsgs = Enumerable.Range(1, 450)
                .Select(x => new UdfTagMsgModel
                {
                    TagId = x,
                    TagStatus = TagStatus.Ok
                });

            var posMsgs = Enumerable.Range(1, 450)
                .Select(x => new UdfPosMsgModel
                {
                    TagId = x,
                    SectorId = 44
                });

            var secMsgs = Enumerable.Range(0, 83)
                .Select(x => new UdfSecMsgModel
                {
                    SectorId = x,
                    SectorStatus = SectorStatus.Ok
                });

            using (var receiver = new DbReceiver(_settings, dbcache))
            using (var sender = new UdfMessageSender(_settings))
            {
                receiver.Start();

                foreach (var tagMsg in tagMsgs)
                {
                    sender.Send(tagMsg);
                }

                foreach (var posMsg in posMsgs)
                {
                    sender.Send(posMsg);
                }

                foreach (var secMsg in secMsgs)
                {
                    sender.Send(secMsg);
                }
                
                Thread.Sleep(1000);

                foreach (var tagMsg in tagMsgs)
                {
                    Assert.AreEqual(tagMsg.TagStatus, dbcache.GetTagStatus(tagMsg.TagId), "Tag status was not received");
                }
                
                //Assert.AreEqual(posMsg.SectorId, dbcache.GetTagSector(posMsg.TagId), "Tag's sector is not correct");
                //Assert.AreEqual(secMsg.SectorStatus, dbcache.GetSectorStatus(secMsg.SectorId), "Sector status was not received");
            }
        }
    }
}