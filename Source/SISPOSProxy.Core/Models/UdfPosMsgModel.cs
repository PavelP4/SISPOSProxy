﻿using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public class UdfPosMsgModel
    {
        public UdfMessageType MessageType => UdfMessageType.PosMsg;
        public int TagId { get; set; }
        public int SectorId { get; set; }
    }
}