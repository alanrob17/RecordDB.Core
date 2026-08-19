using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecordDB.DAL.Repositories
{
    public interface IDiscRepository
    {
        Task<IEnumerable<ArtistRecordDiscDto>> SelectAllDiscEntitiesAsync();
        Task<IEnumerable<ArtistRecordDiscDto>> GetDiscRecordsByRecordNameAsync(string recordName);
        Task<ArtistRecordDiscDto?> SelectSingleDiscAsync(int discId);
        Task<int> InsertDiscAsync(Disc disc);
        Task<int> UpdateDiscAsync(Disc disc);
        Task DeleteDiscAsync(int discId);
        Task UpdateDiscLengthAsync(int discId, int? totalLength);
    }
}
