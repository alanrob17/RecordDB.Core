using RecordDB.DAL.DTOs;
using RecordDB.DAL.Extensions;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecordDB.Test.Services
{
    public class RecordService : IRecordService
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly ITotalRepository _totalRepository;
    
        public RecordService(IRecordRepository recordRepository, IArtistRepository artistRepository, ITotalRepository totalRepository)
        {
            _recordRepository = recordRepository;
            _artistRepository = artistRepository;
            _totalRepository = totalRepository;
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
            // await PrintRecordListAsynch();
            // await GetRecordedYearNumberAsync();
            // await GetRecordsByYearAsync(1974);
            // await NoRecordReviewsAsync();
            // await GetArtistRecords();
            // await GetRecordsByArtistNameAsync("Bob Dylan");

            // TODO: This uses Heinemnann's ToShortDate method, will only work in Windows. Needs to be migrated.
            // ToShortDate();
            await GetTotalsAsync();
            // await InsertRecordAsync();
            // await InsertRecord2Async();
            // await UpdateRecordAsync();  
            // await UpdateRecord2Async();
            // await DeleteRecordAsync();  

            // await GetTotalCosts();
        }

        public async Task PrintRecordListAsynch()
        {
            var artists = await _artistRepository.SelectAsync();
            var records = await _recordRepository.SelectAsync();

            foreach (var artist in artists)
            {
                // Map ArtistRecordDto to Record
                var artistRecords = records
                    .Where(r => r.ArtistId == artist.ArtistId)
                    .Select(dto => new Record
                    {
                        RecordId = dto.RecordId,
                        ArtistId = dto.ArtistId,
                        Name = dto.Name,
                        Field = dto.Field,
                        Recorded = dto.Recorded,
                        Label = dto.Label,
                        Pressing = dto.Pressing,
                        Rating = dto.Rating,
                        Discs = dto.Discs,
                        Media = dto.Media,
                        Bought = dto.Bought,
                        Cost = dto.Cost,
                        CoverName = dto.CoverName,
                        Review = dto.Review
                    })
                    .ToList();

                artist.Records = artistRecords;
            }

            foreach (var artist in artists)
            {
                Console.WriteLine($"Artist: {artist.Name} - Records: {artist.Records.Count}");
                foreach (var record in artist.Records)
                {
                    Console.WriteLine($"  RecordId: {record.RecordId}, Name: {record.Name}, Recorded: {record.Recorded}, Media: {record.Media}");
                }
            }
        }

        public async Task GetRecordsByYearAsync(int year)
        {
            var records = await _recordRepository.GetRecordsByYearAsync(year);
            foreach (var record in records)
            {
                Console.WriteLine($"{record.ArtistName}: {record.Name} - {record.Recorded}");
            }
        }

        public async Task GetTotalCosts()
        {
            var totals = await _totalRepository.GetTotalCosts();

            foreach (var artist in totals)
            {
                Console.WriteLine($"{artist.Name}: {artist.TotalDiscs}: {artist.TotalCost:C}\n");
            }
        }

        public async Task DeleteRecordAsync()
        {
            var recordId = 5303;
            await _recordRepository.DeleteAsync(recordId);

            Console.WriteLine("Record deleted");
        }

        public async Task UpdateRecord2Async()
        {
            var date = "21-07-2025";

            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            var record = new ArtistRecordDto
            {
                RecordId = 5301,
                ArtistId = 915,
                Name = "Crying In Paradise",
                Recorded = 1995,
                Label = "Whoppo",
                Pressing = "Aus",
                Field = "Rock",
                Rating = "***",
                Discs = 1,
                Media = "CD",
                Bought = DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal),
                Cost = 19.99m,
                CoverName = null,
                Review = "This is James' thirty-third album. His last before he turned to religion."
            };

            var recId = await _recordRepository.UpdateAsync(record);

            Console.WriteLine(recId);
        }

        public async Task UpdateRecordAsync()
        {
            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            var recordId = 5300;
            var artistId = 915;
            var name = "Horror In Paradise";
            var recorded = 2024;
            var label = "Whoppo Doppo";
            var pressing = "Aus";
            var field = "Rock";
            var rating = "****";
            var discs = 1;
            var media = "CD";
            var date = "28-05-2025";
            var bought = DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            var cost = 12.99m;
            string? coverName = null;
            var review = "This is James' third album. His last before he went mad.";

            recordId = await _recordRepository.UpdateAsync(recordId, artistId, name, field, recorded, label, pressing, rating, discs, media, bought, cost, coverName, review);

            Console.WriteLine(recordId);
        }

        public async Task InsertRecord2Async()
        {
            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            var artistId = 915;
            var name = "Laughs In Paradise";
            var recorded = 2025;
            var label = "Whoppo";
            var pressing = "Au";
            var field = "Rock";
            var rating = "****";
            var discs = 1;
            var media = "CD";
            var date = "28-05-2025";
            var bought = DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            var cost = 13.99m;
            string? coverName = null;
            var review = "This is James' second album.";

            var recordId = await _recordRepository.InsertAsync(artistId, name, field, recorded, label, pressing, rating, discs, media, bought, cost, coverName, review);

            Console.WriteLine(recordId);
        }

        public async Task InsertRecordAsync()
        {
            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var date = "06-07-2025";

            var record = new Record
            {
                ArtistId = 915,
                Name = "Rockin'n In Paradise",
                Recorded = 2025,
                Label = "Dapper",
                Pressing = "Aus",
                Field = "Jazz",
                Rating = "***",
                Discs = 1,
                Media = "CD",
                Bought = DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal),
                Cost = 19.99m,
                CoverName = null,
                Review = "This is James' Fourth album."
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
                Artist artist = await _artistRepository.SelectAsync(record.ArtistId);
                Console.WriteLine($"{artist.Name} -- {record.Name} {record.Recorded} - {record.Media} : {record.Bought.ToShortDate()}\n");
            }
        }

        public async Task SelectRecordsAsync()
        {
            var records = await _recordRepository.SelectAsync();

            foreach (var record in records)
            {
                Artist artist = await _artistRepository.SelectAsync(record.ArtistId);
                Console.WriteLine($"{artist.Name} -- {record.Name} {record.Recorded} - {record.Media}\n");
            }
        }

        public async Task GetFormattedRecordAsync()
        {
            var recordId = 212;
            ArtistRecordDto record = await _recordRepository.SelectAsync(recordId);
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

            // var artist = await _artistRepository.GetArtistByRecordIdAsync(recordId);
            // var biography = await _ar.GetBiographyAsync(recordId); // not needed
            ArtistRecordDto record = await _recordRepository.SelectAsync(recordId);

            //Console.WriteLine($"\n{artist.ArtistId}: - Artist {artist.Name}:\n");

            Console.WriteLine($"\nArtistId: {record.ArtistId} - Artist: {record.ArtistName} -- RecordId: {record.RecordId}\nName: {record.Name}\nField: {record.Field}\nRecorded: {record.Recorded}\nLabel: {record.Label}\nPressing: {record.Pressing}\nDiscs: {record.Discs}\nMedia: {record.Media}\nBought: {record.Bought.ToShortDate() ?? null}\nCost: ${record.Cost:0.00}\nReview:\n{record.Review}\n\nBiography:\n{record.Biography}");
        }

        public async Task GetArtistRecords()
        {
            var artistId = 114;
            var records = await _recordRepository.GetArtistRecordsAsync(artistId);

            if (!records.Any())
            {
                Console.WriteLine($"No records found for ArtistId: {artistId}");
                return;
            }

            var artist = records.First().Artist;

            Console.WriteLine($"Id: {artist?.ArtistId} - Artist: {artist?.Name}\n");

            foreach (var record in records)
            {
                Console.WriteLine($"{record.RecordId} -- {record.Name}");
            }
        }

        public async Task GetRecordsByArtistNameAsync(string name)
        {
            var records = await _recordRepository.GetRecordsByArtistNameAsync(name);

            if (!records.Any())
            {
                Console.WriteLine($"No records found for Artist: {name}");
                return;
            }

            Console.WriteLine($"Id: {records.First().ArtistId} - Artist: {records.First().ArtistName}\n");

            foreach (var record in records)
            {
                Console.WriteLine($"{record.RecordId} -- {record.Name}");
            }
        }

        public async Task<string> ToStringAsync(ArtistRecordDto record)
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
