using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;

namespace SISPOSProxy.Core.Services
{
    public class UdpRepairService
    {
        private static readonly byte SentenceBegin = Encoding.ASCII.GetBytes("$")[0];
        private static readonly byte[] SentenceEnd = Encoding.ASCII.GetBytes("\r\n");

        private static readonly byte[][] FixingSentenceTypeBytes =
        {
            Encoding.ASCII.GetBytes("$PANSPT")
        };

        private readonly DbCache _dbCache;

        public UdpRepairService(DbCache dbCache)
        {
            _dbCache = dbCache;
        }

        public byte[] FixMessage(byte[] input)
        {
            if (!IsNeededToBeFixed(input)) return input;

            byte[] result;

            try
            {
                result = FixSentences(input);
            }
            catch 
            {
                result = new byte[0];
            }

            return result;
        }

        public bool IsNeededToBeFixed(byte[] input)
        {
            if (input.Length == 0) return false;

            return FixingSentenceTypeBytes.Any(input.ContainsSubArray);
        }

        private byte[] FixSentences(byte[] input)
        {
            var result = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                result[i] = input[i];

                if (input[i] == SentenceBegin)
                {
                    var sntSgmt = GetSentenceFrom(input, i);

                    if (TryFixSentence(sntSgmt))
                    {
                        i = sntSgmt.Count;
                    }
                }
            }

            return result;
        }

        private bool TryFixSentence(ArraySegment<byte> smgt)
        {
            var sntType = GetSentenceType(smgt);

            switch (sntType)
            {
                case UdpSentenceType.PANSPT:
                    break;
            }

            return false;
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
            return UdpSentenceType.Undefined;
        }
    }
}
