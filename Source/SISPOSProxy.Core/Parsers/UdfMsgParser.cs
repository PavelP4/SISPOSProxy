using System;
using System.Collections.Generic;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Parsers
{
    abstract class UdfMsgParser
    {
        private readonly Dictionary<UdfMessageType, byte[]> _dict = new Dictionary<UdfMessageType, byte[]>()
        {
            [UdfMessageType.TagMsg] = new [] { Convert.ToByte('T'), Convert.ToByte('A'), Convert.ToByte('G'), (byte)0 },
            [UdfMessageType.PosMsg] = new [] { Convert.ToByte('P'), Convert.ToByte('O'), Convert.ToByte('S'), (byte)0 },
            [UdfMessageType.SecMsg] = new [] { Convert.ToByte('S'), Convert.ToByte('E'), Convert.ToByte('C'), (byte)0 },
        };

        protected virtual UdfMessageType GetMessageType(byte[] msg)
        {
            foreach (var item in _dict)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (msg[i] != item.Value[i]) break;

                    if (i == 2) return item.Key;
                }
            }

            return UdfMessageType.Undefined;
        }
    }
}
