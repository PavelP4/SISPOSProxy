using System;
using System.Text;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;

namespace SISPOSProxy.Core.Models
{
    public class UdpPanSpt : UdpSentence
    {
        public UdpPanSpt()
            :base(UdpSentenceType.PANSPT)
        {
        }
       
        public int SectorId { get; set; }
        public int TagsCount { get; set; }
        public SectorStatus SectorStatus { get; set; }


        public static bool TryParse(ArraySegment<byte> input, out UdpPanSpt result)
        {
            result = new UdpPanSpt();

            if (input.Array == null || GetSentenceType(input) != result.Type) return false;

            var sepCount = 0;
            var seps = new int[4];

            for (int i = 0, j = input.Offset + i; i < input.Count; i++)
            {
                if (input.Array[j] == SentenceSeparator || input.Array[j] == SentenceChecksumSeparator)
                {
                    sepCount++;
                    seps[sepCount - 1] = j;

                    switch (sepCount)
                    {
                        case 2: // Sector
                            result.SectorId = input.Array.ConvertToIntAsString(seps[0] + 1, seps[1] - seps[0] - 1);
                            break;
                        case 3: // Number of tags
                            result.TagsCount = input.Array.ConvertToIntAsString(seps[1] + 1, seps[2] - seps[1] - 1);
                            break;
                        case 4: // Sector status
                            result.SectorStatus = (SectorStatus)input.Array.ConvertToIntAsString(seps[2] + 1, seps[3] - seps[2] - 1);
                            break;
                    }
                }
            }

            return true;
        }

        public override byte[] ToBytes()
        {
            return base.ToBytes();
        }
    }
}
