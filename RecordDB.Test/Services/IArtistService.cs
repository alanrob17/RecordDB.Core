using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.Test.Services
{
    public interface IArtistService
    {
        Task FindAllArtists();
        Task GetArtistsByPartialName(string name);
        Task FindArtist(int artistId);
        Task<int> CreateArtist();
        Task GetArtistsAsync();
        Task GetArtists();
        Task GetArtistListAsync();
        Task GetArtistsWithNoBiographyAsync();
        Task GetArtistWithNoBiographyAsync(string name);
        Task ShowArtistsAsync();
        Task SelectAsync();
        Task GetArtistNamesAsync();
        Task GetSingleArtistAsync(int artistId);
        Task SelectArtistWithNoBioAsync();
        Task InsertAsync();
        Task Insert2Async();
        Task UpdateArtistAsync();
        Task UpdateArtist2Async();
        Task GetArtistIdAsync();
        Task GetArtistId2Async();
        Task UpdateAsync();
        Task Update2Async();
        Task DeleteArtistAsync();
        Task ShowArtistAsync();
        Task GetBiographyAsync();
    }
}
