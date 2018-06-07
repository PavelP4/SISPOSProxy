using System;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Parsers
{
    class UdfSecMsgParser: UdfMsgParser, IParser<byte[], UdfSecMsgModel>
    {
        public bool TryParse(byte[] input, out UdfSecMsgModel result)
        {
            result = null;

            if (input.Length < 12) return false;

            if (GetMessageType(input) != UdfMessageType.SecMsg) return false;

            result = new UdfSecMsgModel
            {
                SectorId = BitConverter.ToInt32(input.Slice(4, 4), 0),
                SectorStatus = (SectorStatus)BitConverter.ToInt32(input.Slice(8, 4), 0)
            };

            return true;
        }
    }
}
