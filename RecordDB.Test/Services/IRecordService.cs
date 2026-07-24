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
    }
}
