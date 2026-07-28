using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetArtistsAsync();
        Task<IEnumerable<Artist>> GetArtistsByPartialNameAsync(string name);
        Task<IEnumerable<Artist>> GetArtists();
        Task<List<Artist>> GetArtistListAsync();
        Task<List<Artist>> SelectAsync();
        Task<Artist> SelectAsync(int artistId);
        Task<List<Artist>> SelectArtistWithNoBioAsync();
        Task<int> InsertAsync(Artist artist);
        Task<int> InsertAsync(string firstName, string lastName, string biography);
        Task<int> UpdateArtistAsync(Artist artist);
        Task<int> UpdateAsync(int artistId, string firstName, string lastName, string name, string biography);
        Task<int> GetArtistIdAsync(string firstName, string lastName);
        Task<int> GetArtistIdAsync(int recordId);
        Task DeleteAsync(int artistId);
        Task<Artist> GetArtistByRecordIdAsync(int recordId);
        Task<string> GetBiographyAsync(int recordId);
    }
}
