using System;
using System.Collections.Generic;
using System.Text;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Parsers
{
    abstract class UdfMsgParser
    {
        private static readonly Dictionary<UdfMessageType, byte[]> _dict = new Dictionary<UdfMessageType, byte[]>()
        {
            [UdfMessageType.TagMsg] = Encoding.ASCII.GetBytes(new[] { 'T', 'A', 'G', (char)0 }),
            [UdfMessageType.PosMsg] = Encoding.ASCII.GetBytes(new[] { 'P', 'O', 'S', (char)0 }),
            [UdfMessageType.SecMsg] = Encoding.ASCII.GetBytes(new[] { 'S', 'E', 'C', (char)0 })
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
