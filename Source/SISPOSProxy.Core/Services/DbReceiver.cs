using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Services
{
    public class DbReceiver : BaseProcessService
    {
        private const int MaxMessageSize = 12;
        private const int ProcessMessagesSleepTimeout = 1;
        private const int ProcessMessagesUnhandledPercent = 25;

        private readonly DbCache _dbCache;
        private readonly MessageCache _inputCache;

        public DbReceiver(Settings settings, DbCache dbCache)
            :base(settings)
        {
            _dbCache = dbCache;

            _inputCache = new MessageCache();
        }

        public override void Start()
        {
            Run(ReceiveMessages, Settings.Udf2ProxyNamedPipeMaxServerInstances);
            Run(ProcessMessages, Settings.Udf2ProxyNamedPipeMaxServerInstances);
        }

        private void Run(Action<object> action, int count)
        {
            for (int i = 0; i < count; i++)
            {
                ServiceTasks.Add(Task.Factory.StartNew(action, Token, TaskCreationOptions.LongRunning));
            }
        }

        private void ReceiveMessages(object obj)
        {
            while (!Token.IsCancellationRequested)
            {
                using (var pipeServer = new NamedPipeServerStream(
                    Settings.Udf2ProxyNamedPipeName, 
                    PipeDirection.In, 
                    Settings.Udf2ProxyNamedPipeMaxServerInstances, 
                    PipeTransmissionMode.Byte))
                {
                    pipeServer.WaitForConnection();

                    using (var br = new BinaryReader(pipeServer, Encoding.ASCII))
                    {
                        _inputCache.Push(br.ReadBytes(MaxMessageSize));
                    }
                }
            }
        }

        private void ProcessMessages(object obj)
        {
            while (!Token.IsCancellationRequested)
            {
                var nextmsg = _inputCache.Pop();

                if (nextmsg.Length == 0)
                {
                    if ((_inputCache.Count / Settings.Udf2ProxyNamedPipeMaxServerInstances) * 100 < ProcessMessagesUnhandledPercent)
                    {
                        Thread.Sleep(ProcessMessagesSleepTimeout);
                    }

                    continue;
                }
                
                if (UdfTagMsg.TryParse(nextmsg, out var tagResult))
                {
                    _dbCache.Save(tagResult);
                }
                else if (UdfPosMsg.TryParse(nextmsg, out var posResult))
                {
                    _dbCache.Save(posResult);
                }
                else if (UdfSecMsg.TryParse(nextmsg, out var secResult))
                {
                    _dbCache.Save(secResult);
                }
            }
        }
    }
}
