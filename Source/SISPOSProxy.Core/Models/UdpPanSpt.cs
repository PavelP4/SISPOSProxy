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

            int prevSep = -1;
            int curSep;
            int valueNum = 0;
            int value;

            for (int i = 0, j = input.Offset + i; i < input.Count; i++, j++)
            {
                if (input.Array[j] == SentenceSeparator || input.Array[j] == SentenceChecksumSeparator)
                {
                    curSep = j;

                    if (prevSep == -1)
                    {
                        prevSep = curSep;
                        continue;
                    }

                    valueNum++;

                    value = input.Array.ConvertToIntAsString(prevSep + 1, curSep - prevSep - 1);

                    switch (valueNum)
                    {
                        case 1: 
                            result.SectorId = value;
                            break;
                        case 2: 
                            result.TagsCount = value;
                            break;
                        case 3:
                            result.SectorStatus = (SectorStatus)value;
                            break;
                        default:
                            throw new ArgumentException("Unexpected input value");
                    }

                    prevSep = curSep;
                }
            }

            return valueNum == 3;
        }

        public override byte[] GetSentenceData()
        {
            var sb = new StringBuilder(12);

            sb.Append(SectorId);
            sb.Append(SentenceSeparatorStr);
            sb.Append(TagsCount);
            sb.Append(SentenceSeparatorStr);
            sb.Append((int)SectorStatus);
          
            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}
