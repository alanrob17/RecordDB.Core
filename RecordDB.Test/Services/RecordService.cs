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

        public RecordService(RecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public async Task RunAsync()
        {
            //await GetRecordAsync();
            // await CountDiscsAsync();
            // await GetArtistRecordNumberAsync();
            // await GetFormattedRecordAsync();
            // await SelectRecordsAsync();
            // await SelectRecordsShow();
            // await SelectRecordsByArtistIdAsync();
            // await SelectRecordReviewsAsync();
            // await GetRecordedYearNumberAsync();
            // await NoRecordReviewsAsync();
            // ToShortDate();
            // await GetTotalsAsync();
            // await InsertRecordAsync();
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

        //public async Task InsertRecordAsync()
        //{
        //    var record = new Record
        //    {
        //        ArtistId = 907,
        //        Name = "Fun In Paradise",
        //        Recorded = 2025,
        //        Label = "Whoppo",
        //        Pressing = "Au",
        //        Field = "Rock",
        //        Rating = "****",
        //        Discs = 1,
        //        Media = "CD",
        //        Bought = new DateTime(2025, 05, 06),
        //        Cost = 10.99m,
        //        CoverName = string.Empty,
        //        Review = "This is James' first album."
        //    };

        //    var recordId = await _recordRepository.InsertAsync(record);

        //    Console.WriteLine($"New Id: {recordId}");
        //}

        //public async Task GetTotalsAsync()
        //{
        //    var artists = await _recordRepository.GetTotalCostsAsync();

        //    foreach (var artist in artists)
        //    {
        //        Console.WriteLine($"{artist.Name}: {artist.TotalDiscs}: {artist.TotalCost:C}\n");
        //    }
        //}

        //public void ToShortDate()
        //{
        //    var dateStr = "28-12-2015";
        //    var myDate = RecordDAL.Extensions.DateTimeExtensions.ToShortDate(dateStr);

        //    Console.WriteLine(myDate);
        //}

        //public async Task NoRecordReviewsAsync()
        //{
        //    List<MissingReviewDto> records = await _recordRepository.NoRecordReviewsAsync();

        //    foreach (var record in records)
        //    {
        //        Console.WriteLine($"{record.RecordId}: {record.Name} - {record.Record}\n");
        //    }
        //}

        //public async Task GetRecordedYearNumberAsync()
        //{
        //    var year = 1974;
        //    var count = await _recordRepository.GetRecordedYearNumberAsync(year);

        //    Console.WriteLine($"Count: {count} discs");
        //}

        //public async Task SelectRecordReviewsAsync()
        //{
        //    var records = await _recordRepository.SelectRecordReviewsAsync();

        //    foreach (var record in records)
        //    {
        //        Console.WriteLine($"{record.ArtistName} -- {record.Name}\n{record.Review}\n");
        //    }
        //}

        //public async Task SelectRecordsByArtistIdAsync()
        //{
        //    var artistId = 114;

        //    var records = await _recordRepository.SelectArtistRecordsAsync(artistId);

        //    foreach (var record in records)
        //    {
        //        Console.WriteLine($"{record.RecordId} -- {record.Name}");
        //    }
        //}

        //public async Task SelectRecordsShow()
        //{
        //    var show = "r1974";

        //    var records = _recordRepository.Select(show);

        //    foreach (var record in records)
        //    {
        //        Console.WriteLine($"{record.ArtistName} -- {record.Name} {record.Recorded} - {record.Media} : {record.Bought:d}\n");
        //    }
        //}

        //public async Task SelectRecordsAsync()
        //{
        //    var records = await _recordRepository.SelectAsync();

        //    foreach (var record in records)
        //    {
        //        Console.WriteLine($"{record.ArtistName} -- {record.Name} {record.Recorded} - {record.Media}\n");
        //    }
        //}

        //public async Task GetFormattedRecordAsync()
        //{
        //    var recordId = 212;
        //    var record = await _recordRepository.SelectAsync(recordId);
        //    var recordDetails = await ToStringAsync(record);

        //    Console.WriteLine(recordDetails);
        //}

        //public async Task GetArtistRecordNumberAsync()
        //{
        //    var artistId = 114;
        //    var count = await _recordRepository.GetArtistNumberOfRecordsAsync(artistId);

        //    Console.WriteLine($"Count: {count} discs");
        //}

        //public async Task CountDiscsAsync()
        //{
        //    var count = await _recordRepository.CountDiscsAsync("dvds");

        //    Console.WriteLine($"Count: {count} DVD's.");

        //    count = await _recordRepository.CountDiscsAsync("cd");

        //    Console.WriteLine($"Count: {count} CD's");
        //}

        //public async Task GetRecordAsync()
        //{
        //    var recordId = 1135;

        //    var artist = await _ar.GetArtistByRecordIdAsync(recordId);
        //    // var biography = await _ar.GetBiographyAsync(recordId); // not needed
        //    var record = await _recordRepository.SelectAsync(recordId);

        //    Console.WriteLine($"\n{artist.ArtistId}: - Artist {artist.Name}:\n");

        //    Console.WriteLine($"\nRecordId: {record.RecordId}\nName: {record.Name}\nField: {record.Field}\nRecorded: {record.Recorded}\nLabel: {record.Label}\nPressing: {record.Pressing}\nDiscs: {record.Discs}\nMedia: {record.Media}\nBought: {record.Bought.ToShortDateString()}\nCost: ${record.Cost}\nReview:\n{record.Review}\n\nBiography:\n{artist.Biography}");
        //}
    }
}
