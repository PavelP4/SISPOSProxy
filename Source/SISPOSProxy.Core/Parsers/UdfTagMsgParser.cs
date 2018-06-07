﻿using System;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Parsers
{
    class UdfTagMsgParser: UdfMsgParser, IParser<byte[], UdfTagMsgModel>
    {
        public bool TryParse(byte[] input, out UdfTagMsgModel result)
        {
            result = null;

            if (input.Length < 12) return false;

            if (GetMessageType(input) != UdfMessageType.TagMsg) return false;

            result = new UdfTagMsgModel
            {
                TagId = BitConverter.ToInt32(input.Slice(4, 4), 0),
                TagStatus = (TagStatus)BitConverter.ToInt32(input.Slice(8, 4), 0)
            };

            return true;
        }
    }
}
