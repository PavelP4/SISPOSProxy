using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPOSProxy.Core.Models
{
    class UdfTagMsgModel
    {
        public string MessageType { get; set; }
        public int TagId { get; set; }
        public int TagStatus { get; set; }
    }
}
