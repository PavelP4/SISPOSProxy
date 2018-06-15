using System.Net;
using System.Net.Sockets;

namespace SISPOSProxy.Tests.Misc
{
    public class UdpPacketSender
    {
        public void Send(IPEndPoint iepoint, byte[] payload)
        {
            using (var client = new UdpClient(iepoint))
            {
                client.Send(payload, payload.Length);
            }
        }
    }
}
