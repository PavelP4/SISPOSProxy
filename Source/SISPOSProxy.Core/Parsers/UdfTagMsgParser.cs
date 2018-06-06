using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Parsers
{
    class UdfTagMsgParser: IParser<byte[], UdfTagMsgModel>
    {
        public bool TryParse(byte[] input, out UdfTagMsgModel result)
        {
            throw new NotImplementedException();
        }
    }
}
