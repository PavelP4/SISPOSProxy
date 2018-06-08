using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPOSProxy.Core.Caches
{
    public class UdpProcessCache
    {
        private readonly ConcurrentQueue<Task<byte[]>> _queue;
        private readonly object _lock = new object();

        public UdpProcessCache()
        {
            _queue = new ConcurrentQueue<Task<byte[]>>();
        }

        public void Push(Task<byte[]> task)
        {
            _queue.Enqueue(task);
        }

        public byte[] Pop()
        {
            lock (_lock)
            {
                if (_queue.TryPeek(out var task) && task.IsCompleted)
                {
                    if (_queue.TryDequeue(out var comletedTask))
                    {
                        return comletedTask.Result;
                    }
                }
            }

            return new byte[0];
        }

        public int Count => _queue.Count;
    }
}
