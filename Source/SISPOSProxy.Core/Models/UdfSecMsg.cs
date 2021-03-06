﻿using System;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public class UdfSecMsg : UdfMessage
    {
        public UdfSecMsg()
            :base(UdfMessageType.SecMsg)
        {
        }
        
        public int SectorId { get; set; }
        public SectorStatus SectorStatus { get; set; }

        public static bool TryParse(byte[] input, out UdfSecMsg result)
        {
            result = new UdfSecMsg();

            if (GetMessageType(input) != result.Type) return false;

            result.SectorId = BitConverter.ToInt32(input, 4);
            result.SectorStatus = (SectorStatus) BitConverter.ToInt32(input, 8);

            return true;
        }

        public override byte[] GetPayload()
        {
            var result = new byte[8]; // int + int = 8

            BitConverter.GetBytes(SectorId).CopyTo(result, 0);
            BitConverter.GetBytes((int)SectorStatus).CopyTo(result, 4);

            return result;
        }
    }
}
