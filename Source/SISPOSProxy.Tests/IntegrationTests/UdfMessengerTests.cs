using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Helpers;
using SISPOSProxy.Core.Services;

namespace SISPOSProxy.Tests.IntegrationTests
{
    [TestFixture]
    public class UdfMessengerTests
    {
        private readonly Settings _settings;

        public UdfMessengerTests()
        {
            _settings.Udf2ProxyNamedPipeName = "SIS_POS_UDF_Messenger";
        }

        [Test]
        public void ReceiveUdfPosMsg()
        {
            var sectorId = 33;
            var tagId = 25;
            var dbcache = new DbCache();


            using (var dbreceiver = new DbReceiver(_settings, dbcache))
            {
                dbreceiver.Start();

                Thread.Sleep(100);

                using (var conn = DbConnection.NewInstance())
                {
                    var cmd = new MySqlCommand("insert into pos(sectorid, tagid, tagstatus, datetime) values(@sectorid, @tagId, 1, NOW())", conn);

                    cmd.Parameters.AddWithValue("@sectorid", sectorId);
                    cmd.Parameters.AddWithValue("@tagId", tagId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                Thread.Sleep(100);
            }

            Assert.AreEqual(sectorId, dbcache.GetTagSector(tagId));
        }

        [Test]
        public void ReceiveUdfTagMsg()
        {
            var tagId = 25;
            var tagStatus = TagStatus.Ok;
            var dbcache = new DbCache();


            using (var dbreceiver = new DbReceiver(_settings, dbcache))
            {
                dbreceiver.Start();

                Thread.Sleep(100);

                using (var conn = DbConnection.NewInstance())
                {
                    var cmd = new MySqlCommand("update tags set status = @status where id = @id", conn);

                    cmd.Parameters.AddWithValue("@status", (int)tagStatus);
                    cmd.Parameters.AddWithValue("@id", tagId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                Thread.Sleep(100);
            }

            Assert.AreEqual(tagStatus, dbcache.GetTagStatus(tagId));
        }

        [Test]
        public void ReceiveUdfSecMsg()
        {
            var sectorId = 33;
            var sectorStatus = SectorStatus.Ok;
            var dbcache = new DbCache();


            using (var dbreceiver = new DbReceiver(_settings, dbcache))
            {
                dbreceiver.Start();

                Thread.Sleep(100);

                using (var conn = DbConnection.NewInstance())
                {
                    var cmd = new MySqlCommand("update sector_status set status = @status where sectorid = @sectorid", conn);

                    cmd.Parameters.AddWithValue("@status", (int)sectorStatus);
                    cmd.Parameters.AddWithValue("@sectorid", sectorId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                Thread.Sleep(100);
            }

            Assert.AreEqual(sectorStatus, dbcache.GetSectorStatus(sectorId));
        }
    }
}
