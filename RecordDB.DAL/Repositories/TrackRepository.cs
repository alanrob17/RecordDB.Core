using Dapper;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecordDB.DAL.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly IDataAccess _db;

        public TrackRepository(IDataAccess db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectAllTrackEntitiesAsync()
        {
            string sproc = "adm_SelectAllTracks";
            return await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, new { });
        }
        
        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectArtistRecordTracksAsync(string name)
        {
            string sproc = "up_GetTrackListing";
            return await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, new { Name = name });
        }

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByRecordAsync(string name)
        {
            string sproc = "up_GetArtistRecordTracks";
            return await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, new { Name = name });
        }

        public async Task<int> GetTrackNumberAsync(int recordId)
        {
            string sproc = "up_GetNumberOfTracks";
            var result = await _db.GetData<int, dynamic>(sproc, new { RecordId = recordId });
            return result.FirstOrDefault();
        }

        public async Task<int> InsertTrackAsync(Track track)
        {
            var trackId = -1;
            string sproc = "up_InsertTrack";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DiscId", track.DiscId);
                parameters.Add("@TrackNo", track.TrackNo);
                parameters.Add("@Name", track.Name);
                parameters.Add("@TrackLength", track.TrackLength);
                parameters.Add("@Extended", track.Extended);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                trackId = await _db.SaveDataReturnId(sproc, parameters);
                return trackId;
            }
            catch (Exception)
            {
                return trackId;
            }
        }

        public async Task<int> UpdateTrackAsync(Track track)
        {
            var trackId = -1;
            string sproc = "up_UpdateTrack";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TrackId", track.TrackId);
                parameters.Add("@TrackNo", track.TrackNo);
                parameters.Add("@Name", track.Name);
                parameters.Add("@TrackLength", track.TrackLength);
                parameters.Add("@Extended", track.Extended);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                trackId = await _db.SaveDataReturnId(sproc, parameters);
                return trackId;
            }
            catch (Exception)
            {
                return trackId;
            }
        }

        public async Task DeleteTrackAsync(int trackId)
        {
            string sproc = "up_DeleteTrack";
            await _db.SaveData(sproc, new { TrackId = trackId });
        }

        public async Task<ArtistRecordDiscTrackDto> SelectTrackByIdAsync(int trackId)
        {
            string sproc = "up_SelectSingleTrack";
            var result = await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, new { TrackId = trackId });
            return result.FirstOrDefault();
        }
    }
}
