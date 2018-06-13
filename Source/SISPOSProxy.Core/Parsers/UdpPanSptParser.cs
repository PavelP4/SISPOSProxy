using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Parsers
{
    public class UdpPanSptParser : IParser<ArraySegment<byte>, UdpPanSptModel>
    {
        public bool TryParse(ArraySegment<byte> input, out UdpPanSptModel result)
        {
            if (smgt.Array == null) return;

            var separatorCount = 0;
            var separators = new int[3];

            for (int i = 0, j = smgt.Offset + i; i < smgt.Count; i++)
            {
                if (smgt.Array[j] == SentenceSeparator)
                {
                    separatorCount++;
                    separators[separatorCount - 1] = j;

                    outStream.WriteByte(smgt.Array[j]);

                    switch (separatorCount)
                    {
                        case 2: // Sector
                            break;
                        case 3: // Number of tags
                            break;
                    }
                }

                if (smgt.Array[j] == SentenceChecksumSeparator)
                {

                }

                outStream.WriteByte(smgt.Array[j]);
            }
        }
    }
}
