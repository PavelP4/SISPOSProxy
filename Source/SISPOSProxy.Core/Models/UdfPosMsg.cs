using System;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public class UdfPosMsg : UdfMessage
    {
        public UdfPosMsg()
            :base(UdfMessageType.PosMsg)
        {
        }
       
        public int TagId { get; set; }
        public int SectorId { get; set; }

        public static bool TryParse(byte[] input, out UdfPosMsg result)
        {
            result = new UdfPosMsg();

            if (GetMessageType(input) != result.Type) return false;

            result.TagId = BitConverter.ToInt32(input, 4);
            result.SectorId = BitConverter.ToInt32(input, 8);

            return true;
        }
    }
}
