using RecordDB.DAL.DTOs;
using RecordDB.DAL.Extensions;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RecordDB.Test.Services
{
    public class RecordService : IRecordService
    {
        private readonly RecordRepository _recordRepository;
        private readonly ArtistRepository _artistRepository;
        
        public RecordService(RecordRepository recordRepository, ArtistRepository artistRepository)
        {
            _recordRepository = recordRepository;
            _artistRepository = artistRepository;
        }

        public async Task RunAsync()
        {
            // await GetRecordAsync();
            // await CountDiscsAsync();
            // await GetArtistRecordNumberAsync();
            // await GetFormattedRecordAsync();
            // await SelectRecordsAsync();
            // await SelectRecordsShow();
            // await SelectRecordsByArtistIdAsync();
            // await SelectRecordReviewsAsync();
            // await GetRecordedYearNumberAsync();
            // await NoRecordReviewsAsync();

            // TODO: This uses Heinemnann's ToShortDate method, will only work in Windows. Needs to be migrated.
            // ToShortDate();
            // await GetTotalsAsync();
            await InsertRecordAsync();
            // await InsertRecord2Async();
            // await UpdateRecordAsync();  
            // await UpdateRecord2Async();
            // await DeleteRecordAsync();  

            // GetTotalCosts();
        }

        //public void GetTotalCosts()
        //{
        //    var totals = _tr.GetTotalCosts();

        //    foreach (var artist in totals)
        //    {
        //        Console.WriteLine($"{artist.Name}: {artist.TotalDiscs}: {artist.TotalCost:C}\n");
        //    }

        //}

        //public async Task DeleteRecordAsync()
        //{
        //    var recordId = 5295;
        //    await _recordRepository.DeleteAsync(recordId);

        //    Console.WriteLine("Record deleted");
        //}

        //public async Task UpdateRecord2Async()
        //{
        //    var date = "21-06-2025";

        //    IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;

        //    var record = new Record
        //    {
        //        RecordId = 5296,
        //        ArtistId = 907,
        //        Name = "Laughing In Paradise",
        //        Recorded = 1991,
        //        Label = "Whoppo DoDah",
        //        Pressing = "Eng",
        //        Field = "Soundtrack",
        //        Rating = "****",
        //        Discs = 3,
        //        Media = "CD",
        //        Bought = DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal),
        //        Cost = 15.99m,
        //        CoverName = string.Empty,
        //        Review = "This is James' third album. His last before he turned to religion."
        //    };

        //    var recId = await _recordRepository.UpdateAsync(record);

        //    Console.WriteLine(recId);
        //}

        public async Task UpdateRecordAsync()
        {
            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            var recordId = 5296;
            var artistId = 907;
            var name = "Laughter In Paradise";
            var recorded = 2026;
            var label = "Whoppo";
            var pressing = "Eng";
            var field = "Jazz";
            var rating = "***";
            var discs = 2;
            var media = "CD";
            var date = "28-03-2026";
            var bought = DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            var cost = 12.99m;
            var coverName = string.Empty;
            var review = "This is James' third album. His last before he went mad.";

            recordId = await _recordRepository.UpdateAsync(recordId, artistId, name, field, recorded, label, pressing, rating, discs, media, bought, cost, coverName, review);

            Console.WriteLine(recordId);
        }

        public async Task InsertRecord2Async()
        {
            var artistId = 907;
            var name = "Laughs In Paradise";
            var recorded = 2025;
            var label = "Whoppo";
            var pressing = "Au";
            var field = "Rock";
            var rating = "****";
            var discs = 1;
            var media = "CD";
            var bought = new DateTime(2025, 11, 06);
            var cost = 13.99m;
            var coverName = string.Empty;
            var review = "This is James' second album.";

            var recordId = await _recordRepository.InsertAsync(artistId, name, field, recorded, label, pressing, rating, discs, media, bought, cost, coverName, review);

            Console.WriteLine(recordId);
        }

        public async Task InsertRecordAsync()
        {
            var record = new Record
            {
                ArtistId = 907,
                Name = "Fun In Paradise",
                Recorded = 2025,
                Label = "Whoppo",
                Pressing = "Au",
                Field = "Rock",
                Rating = "****",
                Discs = 1,
                Media = "CD",
                Bought = "06-05-2025",
                Cost = 10.99m,
                CoverName = string.Empty,
                Review = "This is James' first album."
            };

            var recordId = await _recordRepository.InsertAsync(record);

            Console.WriteLine($"New Id: {recordId}");
        }

        public async Task GetTotalsAsync()
        {
            var artists = await _recordRepository.GetTotalCostsAsync();

            foreach (var artist in artists)
            {
                Console.WriteLine($"{artist.Name}: {artist.TotalDiscs}: {artist.TotalCost:C}\n");
            }
        }

        public void ToShortDate()
        {
            var dateStr = "28-12-2015";
            var myDate = DAL.Extensions.DateTimeExtensions.ToShortDate(dateStr);

            Console.WriteLine(myDate);
        }

        public async Task NoRecordReviewsAsync()
        {
            List<MissingReviewDto> records = await _recordRepository.NoRecordReviewsAsync();

            foreach (var record in records)
            {
                Console.WriteLine($"{record.RecordId}: {record.Name} - {record.Record}\n");
            }
        }

        public async Task GetRecordedYearNumberAsync()
        {
            var year = 1974;
            var count = await _recordRepository.GetRecordedYearNumberAsync(year);

            Console.WriteLine($"Count: {count} discs");
        }

        public async Task SelectRecordReviewsAsync()
        {
            IEnumerable<RecordReviewDto> records = await _recordRepository.SelectRecordReviewsAsync();

            foreach (var record in records)
            {
                Console.WriteLine($"{record.Name} -- {record.Title}\n{record.Review}\n");
            }
        }

        public async Task SelectRecordsByArtistIdAsync()
        {
            var artistId = 114;

            var records = await _recordRepository.SelectArtistRecordsAsync(artistId);

            foreach (var record in records)
            {
                Console.WriteLine($"{record.RecordId} -- {record.Name}");
            }
        }

        public async Task SelectRecordsShow()
        {
            var show = "r1974";

            List<Record> records = await _recordRepository.Select(show);

            foreach (var record in records)
            {
                Console.WriteLine($"{record.ArtistName} -- {record.Name} {record.Recorded} - {record.Media} : {record.Bought.ToShortDate()}\n");
            }
        }

        public async Task SelectRecordsAsync()
        {
            var records = await _recordRepository.SelectAsync();

            foreach (var record in records)
            {
                Console.WriteLine($"{record.ArtistName} -- {record.Name} {record.Recorded} - {record.Media}\n");
            }
        }

        public async Task GetFormattedRecordAsync()
        {
            var recordId = 212;
            var record = await _recordRepository.SelectAsync(recordId);
            var recordDetails = await ToStringAsync(record);

            Console.WriteLine(recordDetails);
        }

        public async Task GetArtistRecordNumberAsync()
        {
            var artistId = 114;
            var count = await _recordRepository.GetArtistNumberOfRecordsAsync(artistId);

            Console.WriteLine($"Count: {count} discs");
        }

        public async Task CountDiscsAsync()
        {
            var count = await _recordRepository.CountDiscsAsync("dvds");

            Console.WriteLine($"Count: {count} DVD's.");

            count = await _recordRepository.CountDiscsAsync("cd");

            Console.WriteLine($"Count: {count} CD's");
        }

        public async Task GetRecordAsync()
        {
            var recordId = 1135;

            var artist = await _artistRepository.GetArtistByRecordIdAsync(recordId);
            // var biography = await _ar.GetBiographyAsync(recordId); // not needed
            var record = await _recordRepository.SelectAsync(recordId);

            Console.WriteLine($"\n{artist.ArtistId}: - Artist {artist.Name}:\n");

            Console.WriteLine($"\nRecordId: {record.RecordId}\nName: {record.Name}\nField: {record.Field}\nRecorded: {record.Recorded}\nLabel: {record.Label}\nPressing: {record.Pressing}\nDiscs: {record.Discs}\nMedia: {record.Media}\nBought: {record.Bought?.ToShortDate() ?? null}\nCost: ${record.Cost:0.00}\nReview:\n{record.Review}\n\nBiography:\n{artist.Biography}");
        }

        public async Task<string> ToStringAsync(Record record)
        {
            var str = new StringBuilder();

            str.Append("<strong>RecordId: </strong>" + record.RecordId + "<br/>");
            str.Append("<strong>ArtistId: </strong>" + record.ArtistId + "<br/>");
            str.Append("<strong>ArtistName: </strong>" + record.ArtistName + "<br/>");
            str.Append("<strong>Name: </strong>" + record.Name + "<br/>");
            str.Append("<strong>Field: </strong>" + record.Field + "<br/>");
            str.Append("<strong>Recorded: </strong>" + record.Recorded + "<br/>");
            str.Append("<strong>Label: </strong>" + record.Label + "<br/>");
            str.Append("<strong>Pressing: </strong>" + record.Pressing + "<br/>");
            str.Append("<strong>Rating: </strong>" + record.Rating + "<br/>");
            str.Append("<strong>Discs: </strong>" + record.Discs + "<br/>");
            str.Append("<strong>Media: </strong>" + record.Media + "<br/>");

            if (record.Bought != null)
            {
                str.Append("<strong>Bought: </strong>" + record.Bought.ToShortDate() + "<br/>");
            }

            if (!string.IsNullOrEmpty(record.Cost.ToString()))
            {
                str.Append("<strong>Cost: </strong>" + record.Cost + "<br/>");
            }

            if (!string.IsNullOrEmpty(record.CoverName))
            {
                str.Append("<strong>CoverName: </strong>" + record.CoverName + "<br/>");
            }

            if (!string.IsNullOrEmpty(record.Review))
            {
                str.Append("<strong>Review: </strong>" + record.Review + "<br/>");
            }

            return str.ToString();
        }
    }
}
