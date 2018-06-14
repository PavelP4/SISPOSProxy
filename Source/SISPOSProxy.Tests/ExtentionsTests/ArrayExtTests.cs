using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SISPOSProxy.Core.Extentions;
using NUnit.Framework.Internal;

namespace SISPOSProxy.Tests.ExtentionsTests
{
    [TestFixture]
    public class ArrayExtTests
    {
        private byte[] _source;

        public ArrayExtTests()
        {
            _source = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        [Test]
        public void Slice()
        {
            var result = _source.Slice(5, 2);
           
            Assert.IsTrue(result.Length == 2);
            Assert.IsTrue(result[0] == 6 && result[1] == 7);
        }

        [Test]
        public void Populate()
        {
            var result = new byte[10];
            result.Populate<byte>(5);

            Assert.IsTrue(result.All(x => x == 5));
        }

        [Test]
        public void ContainsSubArray()
        {
            var subarray = new byte[]{3,4,5};
            var subarray2 = new byte[] { 5 };
            var subarray3 = new byte[] { 20, 21 };
            var subarray4 = new byte[] { 1, 4, 5 };

            Assert.IsTrue(_source.ContainsSubArray(subarray));
            Assert.IsTrue(_source.ContainsSubArray(subarray2));
            Assert.IsFalse(_source.ContainsSubArray(subarray3));
            Assert.IsFalse(_source.ContainsSubArray(subarray4));
        }


        [Test]
        public void ConvertToIntAsString()
        {
            var source = Encoding.ASCII.GetBytes("hello world 2018!!!");

            var result = source.ConvertToIntAsString(12, 4);
            

            Assert.AreEqual(2018, result);
        }
    }
}
