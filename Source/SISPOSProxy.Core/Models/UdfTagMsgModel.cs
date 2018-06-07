using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    class UdfTagMsgModel
    {
        public UdfMessageType MessageType => UdfMessageType.TagMsg;
        public int TagId { get; set; }
        public TagStatus TagStatus { get; set; }
    }
}
