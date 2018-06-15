using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SISPOSProxy.Core.Caches
{
    public class UdpPayloadCache: IDisposable
    {
        public static readonly int UdpPayloadLength = 1280;
        public static readonly int MaxPayloadGrowthDegree = 4;

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
            var lastlen = (int) _stream.Length - _splitLens.Sum(x => x);
            var resultSize = lastlen > 0 ? _splitLens.Count + 1 : _splitLens.Count;
            var result = CreateOutArray(resultSize);

            var streamPosOld = _stream.Position;
            _stream.Position = 0;

            for (int i = 0; i < _splitLens.Count; i++)
            {
                _stream.Read(result[i], 0, _splitLens[i]);
            }

            if (lastlen > 0)
            {
                _stream.Read(result[resultSize - 1], 0, lastlen);
            }

            _stream.Position = streamPosOld;

            return result;
        }

        private byte[][] CreateOutArray(int size)
        {
            if (size == 0) size = 1;

            var result = new byte[size][];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new byte[UdpPayloadLength];
            }

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
