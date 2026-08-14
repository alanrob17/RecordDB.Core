using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly IDataAccess _db;

        public TrackRepository(IDataAccess db)
        {
            _db = db;
        }

        //public async Task<IEnumerable<ArtistRecordDiscDto>> SelectAllTrackEntitiesAsync()
        //{
        //    string sproc = "up_SelectAllDiscEntities";
        //    return await _db.GetData<ArtistRecordDiscDto, dynamic>(sproc, new { });
        //}


    }
}
