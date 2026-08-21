using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly IDataAccess _db;
        private readonly IConfiguration _configuration;

        public TrackRepository(IDataAccess db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        private string GetConnectionString() =>
            _configuration.GetConnectionString("RecordDb")
                ?? throw new InvalidOperationException("Connection string 'RecordDb' is not configured.");

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectAllTrackEntitiesAsync()
        {
            string sproc = "adm_SelectAllTracks";
            return await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, new { });
        }
        
        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectArtistRecordTracksAsync(string name)
        {
            string sproc = "up_SelectRecordTracks";
            return await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, new { Name = name });
        }

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByRecordAsync(string name)
        {
            string sproc = "up_GetArtistRecordTracks";
            return await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, new { Name = name });
        }

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByPartialNameAsync(string name)
        {
            string sproc = "up_SelectPartialRecordTracks";
            var parameter = new DynamicParameters();
            parameter.Add("@Name", name);
            return await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, parameter);
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

        public async Task BulkTrackInsertAsync(List<Track> tracks)
        {
            string sproc = "up_InsertTracks";
            //return _db.SaveData(sproc, tracks);

            var trackTable = new DataTable();
            trackTable.Columns.Add("DiscId", typeof(int));
            trackTable.Columns.Add("TrackNo", typeof(int));
            trackTable.Columns.Add("Name", typeof(string));
            trackTable.Columns.Add("TrackLength", typeof(int));
            trackTable.Columns.Add("Extended", typeof(string));

            foreach (var track in tracks)
            {
                trackTable.Rows.Add(
                    track.DiscId,
                    track.TrackNo,
                    track.Name,
                    track.TrackLength,
                    track.Extended
                );
            }

            // Pass as Table-Valued Parameter
            var parameters = new
            {
                Tracks = trackTable.AsTableValuedParameter("dbo.TrackTableType")
            };

            await _db.SaveData(sproc, parameters);
        }

        /// <summary>
        /// Calls up_CheckForTracks and returns the @TrackCount OUTPUT value.
        /// A value > 0 means the disc already has tracks.
        /// </summary>
        public async Task<int> CheckForTracksAsync(int discId)
        {
            string sproc = "up_CheckForTracks";
            var parameters = new DynamicParameters();
            parameters.Add("@DiscId", discId);
            parameters.Add("@TrackCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using SqlConnection connection = new(GetConnectionString());
            await connection.ExecuteAsync(sproc, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@TrackCount");
        }

        /// <summary>
        /// Bulk-inserts a full disc of tracks via up_InsertTracks using the TrackTableType TVP.
        /// </summary>
        public async Task BulkInsertTracksAsync(IEnumerable<Track> tracks)
        {
            string sproc = "up_InsertTracks";

            var trackTable = new DataTable();
            trackTable.Columns.Add("DiscId",      typeof(int));
            trackTable.Columns.Add("TrackNo",     typeof(int));
            trackTable.Columns.Add("Name",        typeof(string));
            trackTable.Columns.Add("TrackLength", typeof(int));
            trackTable.Columns.Add("Extended",    typeof(string));
            
            foreach (var track in tracks)
            {
                trackTable.Rows.Add(
                    track.DiscId,
                    track.TrackNo,
                    track.Name ?? string.Empty,
                    (object?)track.TrackLength ?? DBNull.Value,
                    (object?)track.Extended    ?? DBNull.Value
                );
            }

            var parameters = new
            {
                Tracks = trackTable.AsTableValuedParameter("dbo.TrackTableType")
            };

            await _db.SaveData(sproc, parameters);
        }
    }
}
