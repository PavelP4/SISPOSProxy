using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SISPOSProxy.Core.Cache;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Parsers;

namespace SISPOSProxy.Core.Receivers
{
    class DbReceiver: IDisposable
    {
        private const int Buffer_size = 12;

        private readonly CancellationToken _ct;
        private readonly NamedPipeServerStream _pipeServer;
        private readonly UdfTagMsgParser _udfTagMsgParser;
        private readonly UdfMessageCache _udfMessageCache;

        public DbReceiver(CancellationToken ct)
        {
            _ct = ct;
            _pipeServer = new NamedPipeServerStream(Settings.Udf2ProxyNamedPipeName, PipeDirection.In);
            _udfTagMsgParser = new UdfTagMsgParser();
            _udfMessageCache = new UdfMessageCache();
        }

        public void Run()
        {
            Task.Factory.StartNew(ProcessMessages, _ct, TaskCreationOptions.LongRunning);

            while (!_ct.IsCancellationRequested)
            {
                _pipeServer.WaitForConnection();

                using (var br = new BinaryReader(_pipeServer, Encoding.ASCII))
                {
                    _udfMessageCache.Push(br.ReadBytes(Buffer_size));
                }
            }
        }

        private void ProcessMessages(object obj)
        {
            while (!_ct.IsCancellationRequested)
            {
                var nextmsg = _udfMessageCache.Pop();

                if (nextmsg.Length == 0) continue;

                if (_udfTagMsgParser.TryParse(nextmsg, out var result))
                {

                }
            }
        }

        public void Dispose()
        {
            _pipeServer?.Dispose();
        }
    }
}
