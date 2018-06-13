using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Parsers;

namespace SISPOSProxy.Core.Services
{
    public class UdpRepairService
    {
        private static readonly byte SentenceBegin = Encoding.ASCII.GetBytes("$")[0];
        private static readonly byte[] SentenceEnd = Encoding.ASCII.GetBytes("\r\n");
        private static readonly byte SentenceSeparator = Encoding.ASCII.GetBytes(",")[0];
        private static readonly byte SentenceChecksumSeparator = Encoding.ASCII.GetBytes("*")[0];

        private static readonly int UdpPayloadLength = 1304;

        private static readonly IDictionary<UdpSentenceType, byte[]>  SentenceTypeBytes = new Dictionary<UdpSentenceType, byte[]>()
        {
            [UdpSentenceType.PANSPT] = Encoding.ASCII.GetBytes("$PANSPT,")
        };

        private readonly DbCache _dbCache;

        private readonly UdpPanSptParser _udpPanSptParser;

        public UdpRepairService(DbCache dbCache)
        {
            _dbCache = dbCache;

            _udpPanSptParser = new UdpPanSptParser();
        }

        public byte[][] FixMessage(byte[] input)
        {
            if (!IsNeededToBeFixed(input)) return new [] { input };

            try
            {
                return FixSentences(input);
            }
            catch 
            {
                return new byte[0][];
            }
        }

        public bool IsNeededToBeFixed(byte[] input)
        {
            if (input.Length == 0) return false;

            return SentenceTypeBytes.Any(x => input.ContainsSubArray(x.Value));
        }

        private byte[][] FixSentences(byte[] input)
        {
            using (var outStream = new MemoryStream())
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == SentenceBegin)
                    {
                        var sntSgmt = GetSentenceFrom(input, i);
                        var sntType = GetSentenceType(sntSgmt);

                        if (sntType != UdpSentenceType.Undefined)
                        {
                            FixAndSaveResult(sntSgmt, sntType, outStream);
                        }
                        else
                        {
                            OnlySaveResult(sntSgmt, outStream);
                        }

                        i += sntSgmt.Count - 1;
                    }
                    else
                    {
                        outStream.WriteByte(input[i]);
                    }
                }

                return SplitUdpPayload(outStream);
            }
        }

        private byte[][] SplitUdpPayload(MemoryStream outStream)
        {
            throw new NotImplementedException();
        }

        private void FixAndSaveResult(ArraySegment<byte> smgt, UdpSentenceType type, MemoryStream outStream)
        {
            switch (type)
            {
                case UdpSentenceType.PANSPT:
                    FixPANSPT(smgt, outStream);
                    break;
                default:
                    OnlySaveResult(smgt, outStream);
                    break;
            }
        }

        private void OnlySaveResult(ArraySegment<byte> smgt, MemoryStream outStream)
        {
            if (smgt.Array == null) return;

            outStream.Write(smgt.Array, smgt.Offset, smgt.Count);
        }

        private void FixPANSPT(ArraySegment<byte> smgt, MemoryStream outStream)
        {
            if (smgt.Array == null) return;

            if (_udpPanSptParser.TryParse(smgt, out var model))
            {

            }
        }

        private ArraySegment<byte> GetSentenceFrom(byte[] input, int fromIndex)
        {
            var i = fromIndex;
            var isSntEndFound = false;
            var toIndex = fromIndex;

            while (i < input.Length && !isSntEndFound)
            {
                if (input[i] == SentenceEnd[0])
                {
                    isSntEndFound = true;
                    i++;

                    for (int j = 1; j < SentenceEnd.Length; j++, i++)
                    {
                        if (input[i] != SentenceEnd[j])
                        {
                            isSntEndFound = false;
                            break;
                        }
                    }
                }

                i++;
            }

            if (isSntEndFound)
            {
                toIndex = i - 1;
            }

            return new ArraySegment<byte>(input, fromIndex, toIndex);
        }

        private UdpSentenceType GetSentenceType(ArraySegment<byte> sgmt)
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
