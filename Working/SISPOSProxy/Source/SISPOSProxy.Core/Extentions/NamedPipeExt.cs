using System.IO.Pipes;
using System.Threading;

namespace SISPOSProxy.Core.Extentions
{
    public static class NamedPipeExt
    {
        public static void WaitForConnectionEx(this NamedPipeServerStream stream, ManualResetEvent cancelEvent)
        {
            AutoResetEvent connectEvent = new AutoResetEvent(false);
            stream.BeginWaitForConnection(ac =>
            {
                try
                {
                    stream.EndWaitForConnection(ac);
                }
                catch
                {
                    // ignored
                }

                connectEvent.Set();
            }, null);

            if (WaitHandle.WaitAny(new WaitHandle[] {connectEvent, cancelEvent}) == 1)
            {
                stream.Close();
            }
                
        }
    }
}
