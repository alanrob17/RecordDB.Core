using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

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
            await SelectDiscsAsync();
            await SelectDiscsWithoutLengthsAsync();
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
    }
}
