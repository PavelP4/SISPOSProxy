using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public abstract class UdpSentence
    {
        public static readonly IDictionary<UdpSentenceType, string> SentenceTypeStrings = new Dictionary<UdpSentenceType, string>()
        {
            [UdpSentenceType.PANSPT] = "$PANSPT,"
        };
        public static readonly IDictionary<UdpSentenceType, byte[]> SentenceTypeBytes;

        public static readonly string SentenceBeginStr = "$";
        public static readonly byte SentenceBegin;
        public static readonly string SentenceEndStr = "\r\n";
        public static readonly byte[] SentenceEnd;
        public static readonly string SentenceSeparatorStr = ",";
        public static readonly byte SentenceSeparator;
        public static readonly string SentenceChecksumSeparatorStr = "*";
        public static readonly byte SentenceChecksumSeparator;
        

        public UdpSentenceType Type { get; }

        static UdpSentence()
        {
            SentenceTypeBytes = SentenceTypeStrings.ToDictionary(x => x.Key, v => Encoding.ASCII.GetBytes(v.Value));

            SentenceBegin = Encoding.ASCII.GetBytes(SentenceBeginStr)[0];
            SentenceEnd = Encoding.ASCII.GetBytes(SentenceEndStr);
            SentenceSeparator = Encoding.ASCII.GetBytes(SentenceSeparatorStr)[0];
            SentenceChecksumSeparator = Encoding.ASCII.GetBytes(SentenceChecksumSeparatorStr)[0];
        }

        protected UdpSentence(UdpSentenceType type)
        {
            Type = type;
        }

        public byte[] ToBytes()
        {
            var data = GetSentenceData();
            var checksum = GetCheckSum(data);

            var typeLen = SentenceTypeBytes[Type].Length;
            var resultLen = typeLen + data.Length + 1 + checksum.Length + SentenceEnd.Length;
            var result = new byte[resultLen];

            SentenceTypeBytes[Type].CopyTo(result, 0);
            data.CopyTo(result, typeLen);
            result[typeLen + data.Length] = SentenceChecksumSeparator;
            checksum.CopyTo(result, typeLen + data.Length + 1);
            SentenceEnd.CopyTo(result, typeLen + data.Length + 1 + checksum.Length);

            return result;
        }

        public byte[] GetCheckSum(byte[] data)
        {
            byte checksum = 0;

            for (int i = 1; i < SentenceTypeBytes[Type].Length; i++)
            {
                checksum ^= SentenceTypeBytes[Type][i];
            }

            for (int i = 0; i < data.Length; i++)
            {
                checksum ^= data[i];
            }

            return Encoding.ASCII.GetBytes(checksum.ToString("X2"));
        }

        public virtual byte[] GetSentenceData()
        {
            return new byte[0];
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
