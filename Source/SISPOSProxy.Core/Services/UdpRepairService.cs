using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Extentions;

namespace SISPOSProxy.Core.Services
{
    public class UdpRepairService
    {
        private static readonly byte[][] FixingSentenceTypeBytes =
        {
            Encoding.ASCII.GetBytes("$PANSPT")
        };

        private readonly DbCache _dbCache;

        public UdpRepairService(DbCache dbCache)
        {
            _dbCache = dbCache;
        }

        public byte[] FixMessage(byte[] msg)
        {
            if (!IsNeededToBeFixed(msg)) return msg;
           

            return msg;
        }

        public bool IsNeededToBeFixed(byte[] msg)
        {
            return FixingSentenceTypeBytes.Any(msg.ContainsSubArray);
        }
    }
}
