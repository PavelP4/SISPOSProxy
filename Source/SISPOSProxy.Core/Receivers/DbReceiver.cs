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
    class DbReceiver: IDisposable
    {
        private const int MaxMessageSize = 12;

        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;

        private readonly NamedPipeServerStream _pipeServer;

        private readonly UdfTagMsgParser _udfTagMsgParser;
        private readonly UdfTagMsgParser _udfPosMsgParser;
        private readonly UdfTagMsgParser _udfSecMsgParser;

        private readonly UdfMessageCache _udfMessageCache;

        private readonly DbCache _dbCache = DbCache.Instance;

        public DbReceiver()
        {
            _cts = new CancellationTokenSource();
            _token = _cts.Token;

            _pipeServer = new NamedPipeServerStream(Settings.Udf2ProxyNamedPipeName, PipeDirection.In);//, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            _udfTagMsgParser = new UdfTagMsgParser();
            _udfPosMsgParser = new UdfTagMsgParser();
            _udfSecMsgParser = new UdfTagMsgParser();

            _udfMessageCache = new UdfMessageCache();
        }

        public void Start()
        {
            Task.Factory.StartNew(ReceiveMessages, _token, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(ProcessMessages, _token, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        private void ReceiveMessages(object obj)
        {
            while (!_token.IsCancellationRequested)
            {
                _pipeServer.WaitForConnection();

                using (var br = new BinaryReader(_pipeServer, Encoding.ASCII))
                {
                    _udfMessageCache.Push(br.ReadBytes(MaxMessageSize));
                }
            }
        }

        private void ProcessMessages(object obj)
        {
            while (!_token.IsCancellationRequested)
            {
                var nextmsg = _udfMessageCache.Pop();

                if (nextmsg.Length == 0) continue;

                if (_udfTagMsgParser.TryParse(nextmsg, out var tagResult))
                {
                    _dbCache.Save(tagResult);
                }
                else if(_udfPosMsgParser.TryParse(nextmsg, out var posResult))
                {
                    _dbCache.Save(posResult);
                }
                else if (_udfSecMsgParser.TryParse(nextmsg, out var secResult))
                {
                    _dbCache.Save(secResult);
                }
            }
        }

        public void Dispose()
        {
            _cts.Dispose();
            _pipeServer?.Dispose();
        }
    }
}
