using System;
using System.Collections.Generic;
using System.Text;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public abstract class UdfMessage
    {
        public static readonly IDictionary<UdfMessageType, byte[]> MessageTypeBytes = new Dictionary<UdfMessageType, byte[]>()
        {
            [UdfMessageType.TagMsg] = Encoding.ASCII.GetBytes(new[] { 'T', 'A', 'G', (char)0 }),
            [UdfMessageType.PosMsg] = Encoding.ASCII.GetBytes(new[] { 'P', 'O', 'S', (char)0 }),
            [UdfMessageType.SecMsg] = Encoding.ASCII.GetBytes(new[] { 'S', 'E', 'C', (char)0 })
        };

        public UdfMessageType Type { get; }

        public UdfMessage(UdfMessageType type)
        {
            Type = type;
        }

        protected static UdfMessageType GetMessageType(byte[] msg)
        {
            foreach (var item in MessageTypeBytes)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (msg[i] != item.Value[i]) break;

                    if (i == 2) return item.Key;
                }
            }

            return UdfMessageType.Undefined;
        }

        public byte[] GetTypeBytes()
        {
            return MessageTypeBytes[Type];
        }

        public virtual byte[] GetPayload()
        {
            return new byte[0];
        }

        public byte[] ToBytes()
        {
            var type = GetTypeBytes();
            var payload = GetPayload();
            var result = new byte[type.Length + payload.Length];

            type.CopyTo(result, 0);
            payload.CopyTo(result, type.Length);

            return result;
        }
    }
}
