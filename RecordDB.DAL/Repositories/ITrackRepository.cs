using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public interface ITrackRepository
    {
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectAllTrackEntitiesAsync();
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectArtistRecordTracksAsync(int recordId);
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByRecordAsync(string name);
        Task<int> GetTrackNumberAsync(int recordId);
        Task<int> InsertTrackAsync(Track track);
        Task<int> UpdateTrackAsync(Track track);
        Task DeleteTrackAsync(int trackId);
    }
}
