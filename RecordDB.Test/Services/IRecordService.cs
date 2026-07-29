using RecordDB.DAL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.Test.Services
{
    public interface IRecordService
    {
        Task GetRecordAsync();
        Task CountDiscsAsync();
        public void ToShortDate();
        Task GetArtistRecordNumberAsync();
        Task GetFormattedRecordAsync();
        Task SelectRecordsShow();
        Task SelectRecordsByArtistIdAsync();
        Task SelectRecordReviewsAsync();
        Task GetRecordedYearNumberAsync();
        Task NoRecordReviewsAsync();
        Task GetArtistRecords();
        Task DeleteRecordAsync();
        Task UpdateRecord2Async();
        Task UpdateRecordAsync();
        Task InsertRecord2Async();
        Task InsertRecordAsync();
        Task GetTotalsAsync();
        Task SelectRecordsAsync();
        Task GetRecordsByArtistNameAsync(string name);
        Task<string> ToStringAsync(ArtistRecordDto record);
    }
}
