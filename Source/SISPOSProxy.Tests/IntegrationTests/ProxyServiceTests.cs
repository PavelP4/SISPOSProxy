using System;
using System.Net;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Helpers;
using SISPOSProxy.Tests.Misc;

namespace SISPOSProxy.Tests.IntegrationTests
{
    [TestFixture]
    public class ProxyServiceTests
    {
        private readonly int _listenPort = 55554;
        private readonly int _transmittPort = 55555;
        private readonly IPEndPoint _serviceIpEndPoing;

        private readonly UdpPacketSender _udpSender = new UdpPacketSender();
        private readonly UdpPacketReceiver _udpReceiver = new UdpPacketReceiver();

        public ProxyServiceTests()
        {
            _serviceIpEndPoing = new IPEndPoint(NetHelper.GetLocalIPv4(), _listenPort);
        }

        [Test]
        public void ReceiveTransmitt()
        {
            var message = Encoding.ASCII.GetBytes("Hello!!!");

            var t = _udpReceiver.ReceiveAsync(55555);

            Thread.Sleep(100);

            _udpSender.Send(_serviceIpEndPoing, message);

            t.Wait(100);

            Assert.IsNotNull(t.Result);
            Assert.IsTrue(message.ContainsSubArray(t.Result), "Sent and received messages are not equal"); 
        }

        [Test]
        public void RepairMessage()
        {
            var inputWrong = Encoding.ASCII.GetBytes("$PANSPT,33,1,1*24\r\n$PANSPT,35,0,1*23\r\n$PANSSY,0,2,1,1,1,0,0*19\r\n");
            var inputOk = Encoding.ASCII.GetBytes("$PANSPT,33,0,1*25\r\n$PANSPT,35,0,1*23\r\n$PANSSY,0,2,1,1,450,450,0*18\r\n"); // set it after first result will be received 

            var t = _udpReceiver.ReceiveAsync(55555);

            Thread.Sleep(100);

            _udpSender.Send(_serviceIpEndPoing, inputWrong);

            t.Wait(100);

            var resultStr = Encoding.ASCII.GetString(t.Result);

            Assert.IsNotNull(t.Result);
            Assert.IsTrue(!t.Result.ContainsSubArray(inputWrong), "Wrong message was not changed");
            //Assert.IsTrue(t.Result.ContainsSubArray(inputOk), "Wrong message was not corrected");  // uncomment after init inputOk

            Console.Out.WriteLine($"Result: \n{resultStr}");
        }
    }
}
