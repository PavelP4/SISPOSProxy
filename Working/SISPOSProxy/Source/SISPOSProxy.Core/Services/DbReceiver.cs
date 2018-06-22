using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Services
{
    public class DbReceiver : BaseProcessService
    {
        private const int MaxMessageSize = 12;
        private const int ProcessMessagesSleepTimeout = 1;

        private readonly DbCache _dbCache;
        private readonly MessageCache _inputCache;

        private readonly ManualResetEvent _cancelNamedPipeServer;
        private readonly CancellationTokenRegistration _regOfCancelNamedPipeServer;

        public DbReceiver(Settings settings, DbCache dbCache)
            :base(settings)
        {
            _dbCache = dbCache;

            _inputCache = new MessageCache();

            _cancelNamedPipeServer = new ManualResetEvent(false);
            _regOfCancelNamedPipeServer = Token.Register(() => _cancelNamedPipeServer.Set()); 
        }

        public override void Start()
        {
            _cancelNamedPipeServer.Reset();

            Run(ReceiveMessages, Settings.Udf2ProxyNamedPipeMaxServerInstances);
            Run(ProcessMessages, 1);
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
                    PipeTransmissionMode.Byte, 
                    PipeOptions.Asynchronous))
                {
                    pipeServer.WaitForConnectionEx(_cancelNamedPipeServer); 

                    if (Token.IsCancellationRequested) continue;

                    using (var br = new BinaryReader(pipeServer, Encoding.ASCII))
                    {
                        try
                        {
                            _inputCache.Push(br.ReadBytes(MaxMessageSize));
                        }
                        catch
                        {
                            // ignored
                        }
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
                    if (_inputCache.Count == 0)
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

        public override void Dispose()
        {
            _regOfCancelNamedPipeServer.Dispose();
            _cancelNamedPipeServer.Dispose();

            base.Dispose();
        }
    }
}
