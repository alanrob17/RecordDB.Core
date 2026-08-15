using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.Test.Services
{
    public interface ITrackService
    {
        Task SelectTrack();
        Task SelectTracksAsync();
        Task SelectArtistRecordTracksAsync();
        Task SelectTracksByRecordAsync();
        Task GetNumberOfTracksAsync();
        Task InsertTrackAsync();
        Task UpdateTrackAsync();
        Task DeleteTrackAsync();
    }
}
