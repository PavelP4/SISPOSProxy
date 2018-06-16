using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SISPOSProxy.Core.Helpers;

namespace SISPOSProxy.Tests.Misc
{
    public class UdpPacketSender
    {
        public void Send(IPEndPoint iepoint, byte[] payload)
        {
            using (var client = new UdpClient())
            {
                client.Send(payload, payload.Length, iepoint);
            }
        }

        public void SendToLocalhost(int port, byte[] payload)
        {
            var localIpEndPoint = new IPEndPoint(NetHelper.GetLocalIPv4(NetworkInterfaceType.Ethernet), port);

            Send(localIpEndPoint, payload);
        }
    }
}
