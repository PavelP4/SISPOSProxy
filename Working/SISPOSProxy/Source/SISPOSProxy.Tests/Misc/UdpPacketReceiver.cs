using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SISPOSProxy.Tests.Misc
{
    public class UdpPacketReceiver
    {
        public async Task<byte[]> ReceiveAsync(int port)
        {
            return await Task.Run(() =>
            {
                using (var clientS = new UdpClient(port))
                {
                    IPEndPoint ip = null;
                    byte[] data = clientS.Receive(ref ip);
                    return data;
                }
            });
        }
    }
}
