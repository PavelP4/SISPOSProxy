using System;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Parsers
{
    class UdfPosMsgParser: UdfMsgParser, IParser<byte[], UdfPosMsgModel>
    {
        public bool TryParse(byte[] input, out UdfPosMsgModel result)
        {
            result = null;

            if (input.Length < 12) return false;

            if (GetMessageType(input) != UdfMessageType.PosMsg) return false;

            result = new UdfPosMsgModel()
            {
                TagId = BitConverter.ToInt32(input.Slice(4, 4), 0),
                SectorId = BitConverter.ToInt32(input.Slice(8, 4), 0)
            };

            return true;
        }
    }
}
