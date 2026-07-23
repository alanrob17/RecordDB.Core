using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.Test.Services
{
    public interface IArtistService
    {
        Task FindAllArtists();
        Task FindArtist(int artistId);
        Task<int> CreateArtist();
        // TODO: Add methods for updating and deleting artists if needed
    }
}
