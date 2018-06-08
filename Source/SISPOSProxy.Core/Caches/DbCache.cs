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
    public class DbCache
    {
        private const int MaxTagAmount = 451; // Tags 1..450
        private const int MaxSecAmount = 83;  // Sectors 0..82

        private readonly TagStatus[] _tagStatuses = new TagStatus[MaxTagAmount]; 
        private readonly int[] _tagSectors = new int[MaxTagAmount];
        private readonly SectorStatus[] _secStatuses = new SectorStatus[MaxSecAmount]; 

        private readonly object _tagLock = new object();
        private readonly object _secLock = new object();
        

        public DbCache()
        {
            _tagStatuses.Populate(TagStatus.Undefined);
            _tagSectors.Populate(-1);
            _secStatuses.Populate(SectorStatus.Undefined);
        }

        public TagStatus GetTagStatus(int tagId)
        {
            lock (_tagLock)
            {
                return _tagStatuses[tagId];
            }
        }

        public int GetTagSector(int tagId)
        {
            lock (_tagLock)
            {
                return _tagSectors[tagId];
            }
        }

        public int GetSectorTagsAmount(int sectorId)
        {
            lock (_tagLock)
            {
                return _tagSectors.Count(x => x == sectorId);
            }
        }

        public SectorStatus GetSectorStatus(int sectorId) {

            lock (_secLock)
            {
                return _secStatuses[sectorId];
            }
        }

        public void Save(UdfTagMsgModel model)
        {
            if (model.TagId > MaxTagAmount) return;

            lock (_tagLock)
            {
                _tagStatuses[model.TagId] = model.TagStatus;
            }
        }

        public void Save(UdfPosMsgModel model)
        {
            if (model.TagId > MaxTagAmount) return;

            lock (_tagLock)
            {
                _tagSectors[model.TagId] = model.SectorId;
            }
        }

        public void Save(UdfSecMsgModel model)
        {
            if (model.SectorId > MaxSecAmount) return;

            lock (_secLock)
            {
                _secStatuses[model.SectorId] = model.SectorStatus;
            }
        }
    }
}
