using System;
using System.IO;
using System.Linq;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Services
{
    public class UdpRepairService
    {
        private readonly DbCache _dbCache;


        public UdpRepairService(DbCache dbCache)
        {
            _dbCache = dbCache;
        }

        public byte[][] FixPayload(byte[] input)
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

            return UdpSentence.SentenceTypeBytes.Any(x => input.ContainsSubArray(x.Value));
        }

        private byte[][] FixSentences(byte[] input)
        {
            using (var outStream = new UdpPayloadStream())
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == UdpSentence.SentenceBegin)
                    {
                        var sntSgmt = GetSentenceFrom(input, i);

                        FixAndSaveResult(sntSgmt, outStream);

                        i += sntSgmt.Count - 1;
                    }
                    else
                    {
                        outStream.WriteByte(input[i]);
                    }
                }

                return UdpPayloadStream.GetPayloads();
            }
        }

        private ArraySegment<byte> GetSentenceFrom(byte[] input, int fromIndex)
        {
            var i = fromIndex;
            var isSntEndFound = false;
            var toIndex = fromIndex;

            while (i < input.Length && !isSntEndFound)
            {
                if (input[i] == UdpSentence.SentenceEnd[0])
                {
                    isSntEndFound = true;
                    i++;

                    for (int j = 1; j < UdpSentence.SentenceEnd.Length; j++, i++)
                    {
                        if (input[i] != UdpSentence.SentenceEnd[j])
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

        private void FixAndSaveResult(ArraySegment<byte> smgt, UdpPayloadStream outStream)
        {
            UdpSentence baseModel = null;

            try
            {
                if (UdpPanSpt.TryParse(smgt, out var sptModel))
                {
                    FixPANSPT(sptModel);
                    baseModel = sptModel;
                }
                else
                {
                    OnlySaveResult(smgt, outStream);
                }
            
                if (baseModel != null)
                {
                    SaveResult(baseModel.ToBytes(), outStream);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void OnlySaveResult(ArraySegment<byte> smgt, UdpPayloadStream outStream)
        {
            if (smgt.Array == null) return;

            outStream.Write(smgt.Array, smgt.Offset, smgt.Count);
        }

        private void SaveResult(byte[] input, UdpPayloadStream outStream)
        {
            outStream.Write(input, 0, input.Length);
        }

        #region Fixing logic

        private void FixPANSPT(UdpPanSpt model)
        {
            model.TagsCount = _dbCache.GetSectorTagsAmount(model.SectorId);
        }

        #endregion
    }
}
