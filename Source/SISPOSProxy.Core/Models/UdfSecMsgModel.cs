﻿using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    class UdfSecMsgModel
    {
        public UdfMessageType MessageType => UdfMessageType.SecMsg;
        public int SectorId { get; set; }
        public SectorStatus SectorStatus { get; set; }
    }
}
