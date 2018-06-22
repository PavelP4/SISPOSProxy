using NUnit.Framework;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Tests.CachesTests
{
    [TestFixture]
    public class DbCacheTests
    {
        [Test]
        public void InitTags()
        {
            var cache = new DbCache();

            cache.InitTagsAsync().Wait(1000);

            Assert.IsTrue(cache.GetSectorTagsAmount(66) > 0, "Tags sectors were not initialized");
            Assert.IsTrue(cache.GetTagStatus(25) == TagStatus.Ok, "Tag status was not initialized");
        }

        [Test]
        public void InitSectors()
        {
            var cache = new DbCache();

            cache.InitSectorsAsync().Wait(1000);
            
            Assert.IsTrue(cache.GetSectorStatus(66) == SectorStatus.Ok, "Sector status was not initialized");
        }
    }
}
