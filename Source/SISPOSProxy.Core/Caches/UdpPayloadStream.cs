using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPOSProxy.Core.Caches
{
    public class UdpPayloadStream: MemoryStream
    {
        private static readonly int UdpPayloadLength = 1304;

        private byte[][] GetPayloads()
        {
            var splitCount = (int)Math.Ceiling((double)Length / UdpPayloadLength);
            var result = new byte[splitCount][];

            Position = 0;

            for (int i = 0; i < splitCount; i++)
            {
                result[i] = new byte[UdpPayloadLength];
                Read(result[i], UdpPayloadLength * i, UdpPayloadLength);
            }

            return result;
        }
    }
}
