using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPOSProxy.Core.Cache
{
    class UdfMessageCache
    {
        private readonly ConcurrentQueue<byte[]> _queue;

        public UdfMessageCache()
        {
            _queue = new ConcurrentQueue<byte[]>();
        }

        public void Push(byte[] msg)
        {
            _queue.Enqueue(msg);
        }

        public byte[] Pop()
        {
            if (_queue.TryDequeue(out byte[] result))
            {
                return result;
            }

            return new byte[0];
        }
    }
}
