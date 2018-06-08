using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Config;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Tests.Senders
{
    class UdfMessageSender : IDisposable
    {
        private readonly Settings _settings;

        //private readonly NamedPipeClientStream _namedPipeClient;

        public UdfMessageSender(Settings settings)
        {
            _settings = settings;

            //_namedPipeClient = new NamedPipeClientStream(_settings.Udf2ProxyNamedPipeName);
        }

        public void Send(byte[] msg)
        {
            using (var _namedPipeClient = new NamedPipeClientStream(".", _settings.Udf2ProxyNamedPipeName, PipeDirection.Out, PipeOptions.None))
            {
                if (!_namedPipeClient.IsConnected)
                {
                    _namedPipeClient.Connect();
                }

                _namedPipeClient.Write(msg, 0, msg.Length);
                _namedPipeClient.Flush();
                _namedPipeClient.WaitForPipeDrain();
            }
        }

        public void Send(UdfTagMsgModel model)
        {
            var msg = new byte[12];

            var type = new byte[] { Convert.ToByte('T'), Convert.ToByte('A'), Convert.ToByte('G'), 0 };
            var tagId = BitConverter.GetBytes(model.TagId);
            var tagStatus = BitConverter.GetBytes((int)model.TagStatus);
            
            Array.Copy(type, 0, msg, 0, 4);
            Array.Copy(tagId, 0, msg, 4, 4);
            Array.Copy(tagStatus, 0, msg, 8, 4);

            Send(msg);
        }

        public void Send(UdfPosMsgModel model)
        {
            var msg = new byte[12];

            var type = new byte[] { Convert.ToByte('P'), Convert.ToByte('O'), Convert.ToByte('S'), 0 };
            var tagId = BitConverter.GetBytes(model.TagId);
            var sectorId = BitConverter.GetBytes(model.SectorId);

            Array.Copy(type, 0, msg, 0, 4);
            Array.Copy(tagId, 0, msg, 4, 4);
            Array.Copy(sectorId, 0, msg, 8, 4);

            Send(msg);
        }

        public void Send(UdfSecMsgModel model)
        {
            var msg = new byte[12];

            var type = new byte[] { Convert.ToByte('S'), Convert.ToByte('E'), Convert.ToByte('C'), 0 };
            var tagId = BitConverter.GetBytes(model.SectorId);
            var secStatus = BitConverter.GetBytes((int)model.SectorStatus);

            Array.Copy(type, 0, msg, 0, 4);
            Array.Copy(tagId, 0, msg, 4, 4);
            Array.Copy(secStatus, 0, msg, 8, 4);

            Send(msg);
        }

        public void Dispose()
        {
            //_namedPipeClient?.Dispose();
        }
    }
}
