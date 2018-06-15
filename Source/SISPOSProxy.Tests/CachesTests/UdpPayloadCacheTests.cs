using NUnit.Framework;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Extentions;

namespace SISPOSProxy.Tests.CachesTests
{
    [TestFixture]
    public class UdpPayloadCacheTests
    {
        [Test]
        public void PayloadSplit()
        {
            var parts = new[]
            {
                new byte[UdpPayloadCache.UdpPayloadLength - 200],
                new byte[200],
                new byte[UdpPayloadCache.UdpPayloadLength - 400],
                new byte[200],
                new byte[400]
            };

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].Populate((byte)(i + 1));
            }

            using (var cache = new UdpPayloadCache())
            {
                foreach (var part in parts)
                {
                    cache.Write(part, 0, part.Length);
                }

                var result = cache.GetPayloads();

                Assert.AreEqual(result.Length, 3);
                Assert.IsTrue(result[0].ContainsSubArray(parts[0]) && result[0].ContainsSubArray(parts[1]), "First split is wrong");
                Assert.IsTrue(result[1].ContainsSubArray(parts[2]) && result[1].ContainsSubArray(parts[3]) && !result[1].ContainsSubArray(parts[4]), "Second split is wrong");
                Assert.IsTrue(result[2].ContainsSubArray(parts[4]), "Third split is wrong");
            }
        }
    }
}
