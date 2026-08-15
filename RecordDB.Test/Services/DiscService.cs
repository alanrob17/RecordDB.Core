using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecordDB.Test.Services
{
    public class DiscService : IDiscService
    {
        private readonly IDiscRepository _discRepository;

        public DiscService(IDiscRepository discRepository)
        {
            _discRepository = discRepository;
        }

        public async Task RunAsync()
        {
            //await SelectDiscsAsync();
            //await SelectDiscsWithoutLengthsAsync();
            //await GetDiscRecordsByRecordNameAsync("Blonde On Blonde");
            //await SelectSingleDiscAsync(703);
            //await InsertDiscAsync();
            //await UpdateDiscAsync();
            await DeleteDiscAsync();
        }

        public async Task SelectDiscsAsync()
        {
            var discs = await _discRepository.SelectAllDiscEntitiesAsync();

            foreach (var disc in discs)
            {
                // change length into HH:MM:SS
                string length = disc.Length.HasValue ? TimeSpan.FromSeconds(disc.Length.Value).ToString(@"hh\:mm\:ss") : "unk";
                Console.WriteLine($"{disc.ArtistName}: {disc.Name} - {disc.DiscNo} - {length}\n");
            }
        }

        public async Task SelectDiscsWithoutLengthsAsync()
        {
            var discs = await _discRepository.SelectAllDiscEntitiesAsync();
            foreach (var disc in discs)
            {
                if (!disc.Length.HasValue)
                {
                    Console.WriteLine($"{disc.ArtistName}: {disc.Name} - {disc.DiscNo}\n");
                }
            }
        }

        public async Task GetDiscRecordsByRecordNameAsync(string recordName)
        {
            var discs = await _discRepository.GetDiscRecordsByRecordNameAsync(recordName);
            foreach (var disc in discs)
            {
                string length = disc.Length.HasValue ? TimeSpan.FromSeconds(disc.Length.Value).ToString(@"hh\:mm\:ss") : "unk";
                Console.WriteLine($"{disc.ArtistName}: {disc.Name} - {disc.DiscNo} - {disc.FreeDbId} - {length}");
            }
        }

        public async Task SelectSingleDiscAsync(int discId)
        {
            var disc = await _discRepository.SelectSingleDiscAsync(discId);
            if (disc != null)
            {
                string length = disc.Length.HasValue ? TimeSpan.FromSeconds(disc.Length.Value).ToString(@"hh\:mm\:ss") : "unk";
                Console.WriteLine($"{disc.ArtistName}: {disc.Name} - {disc.DiscNo} - {disc.FreeDbId} - {length}");
            }
        }

        private async Task InsertDiscAsync()
        {
            Disc disc = new Disc
            {
                RecordId = 5290,
                DiscNo = 1,
                FreeDbDiscId = 123456,
                FreeDbId = "abc123",
                Length = 3600
            };

            int discId = await _discRepository.InsertDiscAsync(disc);

            if (discId is -1)
            {
                Console.WriteLine("Disc Already exists!");
            }
            else
            {
                Console.WriteLine($"New Disc Id: {discId}");
            }
        }

        public async Task UpdateDiscAsync()
        {
            Disc disc = new Disc
            {
                DiscId = 5610,
                DiscNo = 1,
                FreeDbDiscId = 653456,
                FreeDbId = "zzz12z",
                Length = 6600
            };

            int discId = await _discRepository.UpdateDiscAsync(disc);

            if (discId is -1)
            {
                Console.WriteLine("Disc does not exist!");
            }
            else
            {
                Console.WriteLine($"Updated Disc Id: {discId}");
            }
        }

        public async Task DeleteDiscAsync()
        {
            var discId = 5610;

            await _discRepository.DeleteDiscAsync(discId);
        }

    }
}
