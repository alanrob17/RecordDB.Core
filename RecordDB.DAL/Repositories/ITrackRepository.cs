using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public interface ITrackRepository
    {
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectAllTrackEntitiesAsync();
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectArtistRecordTracksAsync(string name);
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByRecordAsync(string name);
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByPartialNameAsync(string name);
        Task<ArtistRecordDiscTrackDto> SelectTrackByIdAsync(int trackId);
        Task<int> GetTrackNumberAsync(int recordId);
        Task<int> InsertTrackAsync(Track track);
        Task<int> UpdateTrackAsync(Track track);
        Task DeleteTrackAsync(int trackId);
        Task BulkTrackInsertAsync(List<Track> tracks);

        /// <summary>
        /// Checks whether the specified disc already has tracks.
        /// Returns the number of existing tracks (via up_CheckForTracks @TrackCount).
        /// </summary>
        Task<int> CheckForTracksAsync(int discId);

        /// <summary>
        /// Bulk-inserts a full disc of tracks using the TrackTableType TVP
        /// and the up_InsertTracks stored procedure.
        /// </summary>
        Task BulkInsertTracksAsync(IEnumerable<Track> tracks);
    }
}
