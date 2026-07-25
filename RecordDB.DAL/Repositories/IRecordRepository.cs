using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public interface IRecordRepository
    {
        Task<Record> SelectAsync(int recordId);
        Task<string> CountDiscsAsync(string show);
        Task<string> GetArtistNumberOfRecordsAsync(int artistId);
        Task<List<Record>> SelectAsync();
        Task<List<Record>> Select(string show);
        Task<List<Record>> SelectArtistRecordsAsync(int artistId);
        Task<List<RecordReviewDto>> SelectRecordReviewsAsync();
        Task<string> GetRecordedYearNumberAsync(int year);
        Task<List<Total>> GetTotalCostsAsync();
        Task<int> InsertAsync(Record record);

        Task<int> InsertAsync(int artistId, string name, string field, int recorded, string label, string pressing,
            string rating, int discs, string media, DateTime bought, decimal cost, string coverName, string review);

        Task<int> UpdateAsync(int recordId, int artistId, string name, string field, int recorded, string label,
            string pressing, string rating, int discs, string media, DateTime bought, decimal cost, string coverName,
            string review);
        Task<int> UpdateAsync(Record record);
    }
}
