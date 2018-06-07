using System.Collections.Concurrent;

namespace SISPOSProxy.Core.Caches
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
