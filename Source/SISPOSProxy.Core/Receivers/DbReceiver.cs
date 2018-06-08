using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Parsers;

namespace SISPOSProxy.Core.Receivers
{
    public class DbReceiver: IDisposable
    {
        private const int MaxMessageSize = 12;
        private const int ProcessMessagesSleepTimeout = 2;
        private const int ProcessMessagesUnhandledPercent = 25;

        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;

        private readonly UdfTagMsgParser _udfTagMsgParser;
        private readonly UdfPosMsgParser _udfPosMsgParser;
        private readonly UdfSecMsgParser _udfSecMsgParser;

        private readonly UdfMessageCache _udfMessageCache;

        private readonly Settings _settings;
        private readonly DbCache _dbCache;

        public DbReceiver(Settings settings, DbCache dbCache)
        {
            _settings = settings;
            _dbCache = dbCache;

            _cts = new CancellationTokenSource();
            _token = _cts.Token;

            _udfTagMsgParser = new UdfTagMsgParser();
            _udfPosMsgParser = new UdfPosMsgParser();
            _udfSecMsgParser = new UdfSecMsgParser();

            _udfMessageCache = new UdfMessageCache();
        }

        public void Start()
        {
            Run(ReceiveMessages, _settings.Udf2ProxyNamedPipeMaxServerInstances);
            Run(ProcessMessages, _settings.Udf2ProxyNamedPipeMaxServerInstances);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        private void Run(Action<object> action, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Task.Factory.StartNew(action, _token, TaskCreationOptions.LongRunning);
            }
        }

        private void ReceiveMessages(object obj)
        {
            
            while (!_token.IsCancellationRequested)
            {
                using (var pipeServer = new NamedPipeServerStream(
                    _settings.Udf2ProxyNamedPipeName, 
                    PipeDirection.In, 
                    _settings.Udf2ProxyNamedPipeMaxServerInstances, 
                    PipeTransmissionMode.Byte))
                {
                    pipeServer.WaitForConnection();

                    using (var br = new BinaryReader(pipeServer, Encoding.ASCII))
                    {
                        try
                        {
                            _udfMessageCache.Push(br.ReadBytes(MaxMessageSize));
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
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var nextmsg = _udfMessageCache.Pop();

                    if (nextmsg.Length == 0) continue;

                    if (_udfTagMsgParser.TryParse(nextmsg, out var tagResult))
                    {
                        _dbCache.Save(tagResult);
                    }
                    else if (_udfPosMsgParser.TryParse(nextmsg, out var posResult))
                    {
                        _dbCache.Save(posResult);
                    }
                    else if (_udfSecMsgParser.TryParse(nextmsg, out var secResult))
                    {
                        _dbCache.Save(secResult);
                    }
                }
                catch
                {
                    // ignored
                }

                if ((_udfMessageCache.Count /_settings.Udf2ProxyNamedPipeMaxServerInstances)*100 < ProcessMessagesUnhandledPercent)
                {
                   Thread.Sleep(ProcessMessagesSleepTimeout); 
                }
            }
        }

        public void Dispose()
        {
            Stop();

            _cts.Dispose();
        }
    }
}
