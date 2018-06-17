using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SISPOSProxy.Core.Enums;
using SISPOSProxy.Core.Extentions;
using SISPOSProxy.Core.Helpers;
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

        #region Get / Set
        
        public TagStatus GetTagStatus(int tagId)
        {
            lock (_tagLock)
            {
                return _tagStatuses[tagId];
            }
        }

        public void SetTagStatus(int tagId, TagStatus status)
        {
            lock (_tagLock)
            {
                if (_tagStatuses[tagId] != status)
                {
                    _tagSectors[tagId] = GetTagSectorInitialValue(status);
                }

                _tagStatuses[tagId] = status;
            }
        }

        public int GetTagSector(int tagId)
        {
            lock (_tagLock)
            {
                return _tagSectors[tagId];
            }
        }

        public void SetTagSector(int tagId, int sectorId)
        {
            lock (_tagLock)
            {
                _tagSectors[tagId] = sectorId;
            }
        }

        public SectorStatus GetSectorStatus(int sectorId)
        {
            lock (_secLock)
            {
                return _secStatuses[sectorId];
            }
        }

        public void SetSectorStatus(int sectorId, SectorStatus status)
        {
            lock (_secLock)
            {
                _secStatuses[sectorId] = status;
            }
        }

        #endregion

        #region Queries

        public int GetSectorTagsAmount(int sectorId)
        {
            lock (_tagLock)
            {
                return _tagSectors.Count(x => x == sectorId);
            }
        }

        public int GetSectorTagsAmount(int fromSectorId, int toSectorId)
        {
            lock (_tagLock)
            {
                return _tagSectors.Count(x => x >= fromSectorId && x <= toSectorId);
            }
        }

        #endregion

        #region Save from model

        public void Save(UdfTagMsg model)
        {
            if (model.TagId > MaxTagAmount) return;

            SetTagStatus(model.TagId, model.TagStatus);
        }

        public void Save(UdfPosMsg model)
        {
            if (model.TagId > MaxTagAmount) return;

            SetTagSector(model.TagId, model.SectorId);
        }

        public void Save(UdfSecMsg model)
        {
            if (model.SectorId > MaxSecAmount) return;

            SetSectorStatus(model.SectorId, model.SectorStatus);
        }

        #endregion

        #region Init logic
        
        public async Task InitAsync()
        {
            var t1 = InitTagsAsync();
            var t2 = InitSectorsAsync();

            await Task.WhenAll(t1, t2);
        }

        public async Task InitTagsAsync()
        {
            using (var conn = DbConnection.NewInstance())
            {
                var sql = "SELECT id, status + 0 as statusIndex FROM tags";
                var cmd = new MySqlCommand(sql, conn);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        var tagId = reader.GetInt32(0);
                        var tagStatus = (TagStatus)reader.GetInt32(1);
                       
                        SetTagStatus(tagId, tagStatus);
                    }
                }

                await conn.CloseAsync();
            }
        }

        public async Task InitSectorsAsync()
        {
            using (var conn = DbConnection.NewInstance())
            {
                var sql = "SELECT id, transition FROM sectors";
                var cmd = new MySqlCommand(sql, conn);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        var sectorId = reader.GetInt32(0);
                        var transId = reader.GetInt32(1);
                        
                        SetSectorStatus(sectorId, GetSectorStatusInitialValue(transId));
                    }
                }

                await conn.CloseAsync();
            }
        }

        private int GetTagSectorInitialValue(TagStatus status)
        {
            switch (status)
            {
                case TagStatus.Ok:
                    return 66;

                case TagStatus.LoggedOff:
                    return 67;

                case TagStatus.Inactive:
                    return 0;

                default:
                    return -1;
            }
        }

        private SectorStatus GetSectorStatusInitialValue(int transitionId)
        {
            return transitionId == 0 ? SectorStatus.Ok : SectorStatus.Inactive;
        }

        #endregion
    }
}
