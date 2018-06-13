using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Enums;

namespace SISPOSProxy.Core.Models
{
    public abstract class UdpBaseSentenceModel
    {
        public UdpSentenceType Type { get; }

        public UdpBaseSentenceModel(UdpSentenceType type)
        {
            Type = type;
        }

        public abstract byte[] ToBytes();
    }
}
