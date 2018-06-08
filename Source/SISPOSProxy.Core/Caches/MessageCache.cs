using System.Collections.Concurrent;

namespace SISPOSProxy.Core.Caches
{
    public class MessageCache
    {
        private readonly ConcurrentQueue<byte[]> _queue;

        public MessageCache()
        {
            _queue = new ConcurrentQueue<byte[]>();
        }

        public void Push(byte[] msg)
        {
            _queue.Enqueue(msg);
        }

        public byte[] Pop()
        {
            if (_queue.TryDequeue(out var result))
            {
                return result;
            }

            return new byte[0];
        }

        public int Count => _queue.Count;
    }
}
