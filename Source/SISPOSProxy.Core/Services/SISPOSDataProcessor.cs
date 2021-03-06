﻿using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Core.Services
{
    public class SISPOSDataProcessor : BaseProcessService
    {
        private const int MessageProcessSleepTimeout = 1;
        private const int PopulateProcessSleepTimeout = 1;

        private readonly DbCache _dbCache;
        private readonly MessageCache _inputCache;
        private readonly MessageCache _outputCache;
        private readonly UdpRepairService _repairService;
        private readonly UdpProcessCache _processCache;

        public SISPOSDataProcessor(Settings settings, DbCache dbCache, MessageCache inputCache, MessageCache outputCache) 
            : base(settings)
        {
            _dbCache = dbCache;
            _inputCache = inputCache;
            _outputCache = outputCache;

            _repairService = new UdpRepairService(dbCache);
            _processCache = new UdpProcessCache();
        }

        public override void Start()
        {
            ServiceTasks.Add(Task.Factory.StartNew(ProcessPackets, Token, TaskCreationOptions.LongRunning));
            ServiceTasks.Add(Task.Factory.StartNew(PopulateOutputCache, Token, TaskCreationOptions.LongRunning));
        }


        private void ProcessPackets(object obj)
        {
            while (!Token.IsCancellationRequested)
            {
                var nextmsg = _inputCache.Pop();

                if (nextmsg.Length == 0)
                {
                    if (_processCache.Count == 0)
                    {
                        Thread.Sleep(MessageProcessSleepTimeout);
                    }
                    
                    continue;
                }

                _processCache.Push(Task.Run(() => _repairService.FixPayload(nextmsg), Token));
            }
        }

        private void PopulateOutputCache(object obj)
        {
            while (!Token.IsCancellationRequested)
            {
                var nextmsgs = _processCache.Pop();

                if (nextmsgs.Length == 0)
                {
                    if (_processCache.Count == 0)
                    {
                        Thread.Sleep(PopulateProcessSleepTimeout);
                    }
                    
                    continue;
                }

                foreach (var msg in nextmsgs)
                {
                    _outputCache.Push(msg);
                }
            }
        }
    }
}
