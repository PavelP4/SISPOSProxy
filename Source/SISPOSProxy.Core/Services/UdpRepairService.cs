using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Caches;

namespace SISPOSProxy.Core.Services
{
    public class UdpRepairService
    {
        private readonly DbCache _dbCache;

        public UdpRepairService(DbCache dbCache)
        {
            _dbCache = dbCache;
        }

        public byte[] FixMessage(byte[] msg)
        {
            return new byte[0];
        }
    }
}
