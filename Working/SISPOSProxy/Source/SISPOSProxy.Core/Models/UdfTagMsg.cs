﻿using System;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public class UdfTagMsg : UdfMessage
    {
        public UdfTagMsg()
            :base(UdfMessageType.TagMsg)
        {
        }
        
        public int TagId { get; set; }
        public TagStatus TagStatus { get; set; }

        public static bool TryParse(byte[] input, out UdfTagMsg result)
        {
            result = new UdfTagMsg();
            
            if (GetMessageType(input) != result.Type) return false;

            result.TagId = BitConverter.ToInt32(input, 4);
            result.TagStatus = (TagStatus) BitConverter.ToInt32(input, 8);

            return true;
        }

        public override byte[] GetPayload()
        {
            var result = new byte[8]; // int + int = 8

            BitConverter.GetBytes(TagId).CopyTo(result, 0);
            BitConverter.GetBytes((int)TagStatus).CopyTo(result, 4);

            return result;
        }
    }
}
