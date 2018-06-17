using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;

namespace SISPOSProxy.Core.Models
{
    public class UdpPanSsy: UdpSentence
    {
        public SystemStatus SystemStatus { get; set; }
        public PosServerStatus Pos1Status { get; set; }
        public PosServerStatus Pos2Status { get; set; }
        public SilenceStatus SilenceStatus { get; set; }
        public int LoggedOnTagsAmount { get; set; }
        public int MissedTagsAmount { get; set; }
        public IlasstStatus IlasstStatus { get; set; }
        
        public UdpPanSsy() 
            : base(UdpSentenceType.PANSSY)
        {
        }

        public static bool TryParse(ArraySegment<byte> input, out UdpPanSsy result)
        {
            result = new UdpPanSsy();

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
                            result.SystemStatus = (SystemStatus)value;
                            break;
                        case 2:
                            result.Pos1Status = (PosServerStatus)value;
                            break;
                        case 3:
                            result.Pos2Status = (PosServerStatus)value;
                            break;
                        case 4:
                            result.SilenceStatus = (SilenceStatus)value;
                            break;
                        case 5:
                            result.LoggedOnTagsAmount = value;
                            break;
                        case 6:
                            result.MissedTagsAmount = value;
                            break;
                        case 7:
                            result.IlasstStatus = (IlasstStatus)value;
                            break;
                        default:
                            throw new ArgumentException("Unexpected input value");
                    }

                    prevSep = curSep;
                }
            }

            return valueNum == 7;
        }

        public override byte[] GetSentenceData()
        {
            var sb = new StringBuilder(20);

            sb.Append((int)SystemStatus);
            sb.Append(SentenceSeparatorStr);
            sb.Append((int)Pos1Status);
            sb.Append(SentenceSeparatorStr);
            sb.Append((int)Pos2Status);
            sb.Append(SentenceSeparatorStr);
            sb.Append((int)SilenceStatus);
            sb.Append(SentenceSeparatorStr);
            sb.Append(LoggedOnTagsAmount);
            sb.Append(SentenceSeparatorStr);
            sb.Append(MissedTagsAmount);
            sb.Append(SentenceSeparatorStr);
            sb.Append((int)IlasstStatus);

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}
