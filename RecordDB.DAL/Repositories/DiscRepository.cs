using Dapper;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RecordDB.DAL.Repositories
{
    public class DiscRepository : IDiscRepository
    {
        private readonly IDataAccess _db;

        public DiscRepository(IDataAccess db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ArtistRecordDiscDto>> SelectAllDiscEntitiesAsync()
        {
            string sproc = "up_SelectAllDiscEntities";
            return await _db.GetData<ArtistRecordDiscDto, dynamic>(sproc, new { });
        }

        public async Task<IEnumerable<ArtistRecordDiscDto>> GetDiscRecordsByRecordNameAsync(string recordName)
        {
            if (string.IsNullOrWhiteSpace(recordName))
            {
                return Enumerable.Empty<ArtistRecordDiscDto>();
            }

            string sproc = "up_GetDiscRecordsByRecordName";
            var parameters = new DynamicParameters();
            parameters.Add("@Name", recordName);

            return await _db.GetData<ArtistRecordDiscDto, dynamic>(sproc, parameters);
        }

        public async Task<ArtistRecordDiscDto?> SelectSingleDiscAsync(int discId)
        {
            string sproc = "up_SelectSingleDisc";
            var parameters = new DynamicParameters();
            parameters.Add("@DiscId", discId);

            return await _db.GetFirstOrDefault<ArtistRecordDiscDto, dynamic>(sproc, parameters);
        }

        public async Task<int> InsertDiscAsync(Disc disc)
        {
            var discId = -1;
            string sproc = "up_InsertDisc";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RecordId", disc.RecordId);
                parameters.Add("@DiscNo", disc.DiscNo);
                parameters.Add("@FreeDbDiscId", disc.FreeDbDiscId);
                parameters.Add("@FreeDbId", disc.FreeDbId);
                parameters.Add("@Length", disc.Length);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                discId = await _db.SaveDataReturnId(sproc, parameters);
                return discId;
            }
            catch (Exception)
            {
                return discId;
            }
        }

        public async Task<int> UpdateDiscAsync(Disc disc)
        {
            var discId = -1;
            string sproc = "up_UpdateDisc";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DiscId", disc.DiscId);
                parameters.Add("@DiscNo", disc.DiscNo);
                parameters.Add("@FreeDbDiscId", disc.FreeDbDiscId);
                parameters.Add("@FreeDbId", disc.FreeDbId);
                parameters.Add("@Length", disc.Length);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                discId = await _db.SaveDataReturnId(sproc, parameters);
                return discId;
            }
            catch (Exception)
            {
                return discId;
            }
        }

        public async Task DeleteDiscAsync(int discId)
        {
            try
            {
                string sproc = "up_DiscDelete";
                var parameters = new DynamicParameters();
                parameters.Add("@DiscId", discId);

                await _db.SaveData(sproc, parameters);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
