using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SISPOSProxy.Core.Caches
{
    public class UdpPayloadCache: IDisposable
    {
        public static readonly int UdpPayloadLength = 1304;
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
            var lastlen = (int) _stream.Length - _splitLens.Sum();
            var resultSize = lastlen > 0 ? _splitLens.Count + 1 : _splitLens.Count;
            var result = new List<byte[]>(resultSize);

            var streamPosOld = _stream.Position;
            _stream.Position = 0;

            try
            {
                foreach (int len in _splitLens)
                {
                    if (!IsEmptyNextPayload(len))
                    {
                        result.Add(CreatePayload(len));
                    }
                }

                if (lastlen > 0 && !IsEmptyNextPayload(lastlen))
                {
                    result.Add(CreatePayload(lastlen));
                }
            }
            finally
            {
               _stream.Position = streamPosOld; 
            }

            return result.ToArray();
        }

        private byte[] CreatePayload(int length)
        {
            var newPayload = new byte[UdpPayloadLength];

            _stream.Read(newPayload, 0, length);

            return newPayload;
        }

        private bool IsEmptyNextPayload(int length)
        {
            var curpos = _stream.Position;

            if (length == 0) return true;

            try
            {
                for (int i = 0; i < length; i++)
                {
                    if (_stream.ReadByte() != 0) return false;
                }
            }
            finally
            {
                _stream.Position = curpos;
            }

            return true;
        }

        private void CheckSplittingLen(int addLen)
        {
            if (addLen > UdpPayloadLength)
            {
                throw new ArgumentException("Unexpected addLen argument");
            }

            var newSplitLen = (int) (_stream.Length - _splitLens.Sum());

            if (newSplitLen + addLen > UdpPayloadLength)
            {
                _splitLens.Add(newSplitLen);
            }
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
