using System;
using System.Collections.Generic;
using System.IO;

namespace SISPOSProxy.Core.Caches
{
    public class UdpPayloadCache: IDisposable
    {
        private static readonly int UdpPayloadLength = 1304;
        private static readonly int MaxPayloadGrowthDegree = 4;

        private readonly MemoryStream _stream;
        private readonly IList<int> _splitLens;

        public UdpPayloadCache()
        {
            _stream = new MemoryStream();
            _splitLens = new List<int>(MaxPayloadGrowthDegree);
        }

        public void WriteByte(byte input)
        {
            CheckSplittingLen(1);

            _stream.WriteByte(input);
        }

        public void Write(byte[] input, int offset, int count)
        {
            CheckSplittingLen(input.Length - offset - count < 0 ? input.Length - offset : count);

            _stream.Write(input, offset, count);
        }

        public byte[][] GetPayloads()
        {
            var lastlen = (int) _stream.Length - (_splitLens.Count * UdpPayloadLength);
            var resultCount = lastlen > 0 ? _splitLens.Count + 1 : _splitLens.Count;
            var result = new byte[resultCount][];

            var streamPosOld = _stream.Position;
            _stream.Position = 0;

            for (int i = 0; i < _splitLens.Count; i++)
            {
                result[i] = new byte[UdpPayloadLength];
                _stream.Read(result[i], 0, _splitLens[i]);
            }

            if (resultCount > _splitLens.Count)
            {
                _stream.Read(result[resultCount - 1], 0, lastlen);
            }

            _stream.Position = streamPosOld;

            return result;
        }

        private void CheckSplittingLen(int addLen)
        {
            var newSplitLen = (int) _stream.Length - (_splitLens.Count * UdpPayloadLength);

            if (newSplitLen + addLen <= UdpPayloadLength) return;

            if (newSplitLen <= UdpPayloadLength)
            {
                _splitLens.Add(newSplitLen);
            }
            else
            {
                throw new Exception("Unexpected stream length");
            }
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
