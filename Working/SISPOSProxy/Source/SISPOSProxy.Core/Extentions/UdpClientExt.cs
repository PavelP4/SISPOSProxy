using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SISPOSProxy.Core.Extentions
{
    public static class UdpClientExt
    {
        class UdpClientState
        {
            public UdpClient Client { get; set; }
            public AutoResetEvent ConnectEvent { get; set; }
            public byte[] Buffer { get; set; }
        }

        public static byte[] ReceiveEx(this UdpClient client, ManualResetEvent cancelEvent)
        {
            AutoResetEvent connectEvent = new AutoResetEvent(false);
            var state = new UdpClientState()
            {
                Client = client,
                ConnectEvent = connectEvent
            };


            client.BeginReceive(ReceiveCallback, state);

            if (WaitHandle.WaitAny(new WaitHandle[] {connectEvent, cancelEvent}) == 1)
            {
                client.Close();
                return new byte[0];
            }
            
             return state.Buffer;
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            var state = (UdpClientState)ar.AsyncState;
            IPEndPoint remoteEP = null;

            try
            {
                state.Buffer = state.Client.EndReceive(ar, ref remoteEP);
            }
            catch
            {
                state.Buffer = new byte[0];
            }

            state.ConnectEvent.Set();
        }
    }
}
