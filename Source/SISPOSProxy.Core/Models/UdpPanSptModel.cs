using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public class UdpPanSptModel : UdpBaseSentenceModel
    {
        public UdpSentenceType Type => UdpSentenceType.PANSPT;
        public int Sector { get; set; }
        public int TagsCount { get; set; }
        public SectorStatus SectorStatus { get; set; }
    }
}
