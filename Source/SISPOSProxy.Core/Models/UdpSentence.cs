using System;
using System.Collections.Generic;
using System.Text;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public abstract class UdpSentence
    {
        public static readonly IDictionary<UdpSentenceType, byte[]> SentenceTypeBytes = new Dictionary<UdpSentenceType, byte[]>()
        {
            [UdpSentenceType.PANSPT] = Encoding.ASCII.GetBytes("$PANSPT")
        };

        public static readonly byte SentenceBegin = Encoding.ASCII.GetBytes("$")[0];
        public static readonly byte[] SentenceEnd = Encoding.ASCII.GetBytes("\r\n");
        public static readonly byte SentenceSeparator = Encoding.ASCII.GetBytes(",")[0];
        public static readonly byte SentenceChecksumSeparator = Encoding.ASCII.GetBytes("*")[0];

        public UdpSentenceType Type { get; }

        protected UdpSentence(UdpSentenceType type)
        {
            Type = type;
        }

        public virtual byte[] ToBytes()
        {
            return SentenceTypeBytes[Type];
        }

        public static UdpSentenceType GetSentenceType(ArraySegment<byte> sgmt)
        {
            var result = UdpSentenceType.Undefined;

            foreach (var typeBytes in SentenceTypeBytes)
            {
                if (sgmt.Count < typeBytes.Value.Length) continue;

                result = typeBytes.Key;

                for (int i = 0, j = sgmt.Offset + i; i < typeBytes.Value.Length; i++)
                {
                    if (typeBytes.Value[i] != sgmt.Array?[j])
                    {
                        result = UdpSentenceType.Undefined;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
