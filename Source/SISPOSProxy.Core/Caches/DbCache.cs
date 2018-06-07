using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Models;

namespace SISPOSProxy.Core.Caches
{
    class DbCache
    {
        private const int MaxTagAmount = 451; // Tags 1..450
        private const int MaxSecAmount = 83;  // Sectors 0..82

        private readonly TagStatus[] _tagStatus = new TagStatus[MaxTagAmount]; 
        private readonly int[] _tagSector = new int[MaxTagAmount];
        private readonly SectorStatus[] _secStatus = new SectorStatus[MaxSecAmount]; 

        private readonly object _tagLock = new object();
        private readonly object _secLock = new object();

        public static readonly DbCache Instance = new DbCache();

        public DbCache()
        {
            _tagStatus.Populate(TagStatus.Undefined);
            _tagSector.Populate(-1);
            _secStatus.Populate(SectorStatus.Undefined);
        }

        public void Save(UdfTagMsgModel model)
        {
            if (model.TagId > MaxTagAmount) return;

            lock (_tagLock)
            {
                _tagStatus[model.TagId] = model.TagStatus;
            }
        }

        public void Save(UdfPosMsgModel model)
        {
            if (model.TagId > MaxTagAmount) return;

            lock (_tagLock)
            {
                _tagSector[model.TagId] = model.SectorId;
            }
        }

        public void Save(UdfSecMsgModel model)
        {
            if (model.SectorId > MaxSecAmount) return;

            lock (_secLock)
            {
                _secStatus[model.SectorId] = model.SectorStatus;
            }
        }
    }
}
