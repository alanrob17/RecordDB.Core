using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.Test.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task RunAsync()
        {
            // await FindAllArtists();
            // await GetArtistsByPartialName("John");
            //await FindArtist(114);
            //await CreateArtist();
            //await GetArtistsAsync();
            //await GetArtists();
            // await GetArtistListAsync();
            //await GetArtistsWithNoBiographyAsync();
            await GetArtistWithNoBiographyAsync("Jesse Colin Young");

            //await ShowArtistsAsync();
            //await SelectAsync();
            //await GetArtistNamesAsync();
            //await GetSingleArtistAsync(114);
            //await SelectArtistWithNoBioAsync();
            // await InsertAsync();
            // await Insert2Async();
            // await UpdateArtistAsync();
            //await UpdateArtist2Async();
            //await GetArtistIdAsync();
            //await GetArtistId2Async();
            //await UpdateAsync();
            //await Update2Async();
            //await DeleteArtistAsync();
            //await ShowArtistAsync();
            //await GetBiographyAsync();
        }

        public async Task FindAllArtists()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsAsync();

            foreach (Artist artist in artists)
            {
                var biography = string.Empty;
                if (artist.Biography != null && artist.Biography.Length > 60)
                {
                    biography = artist.Biography.Substring(0, 60);
                }

                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}, Biography: {biography}");
            }
        }

        public async Task GetArtistsByPartialName(string name)
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsByPartialNameAsync(name);

            foreach (Artist artist in artists)
            {
                var biography = string.Empty;
                if (artist.Biography != null && artist.Biography.Length > 60)
                {
                    biography = artist.Biography.Substring(0, 60);
                }

                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}, Biography: {biography}");
            }
        }

        public async Task GetArtistsAsync()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsAsync();

            foreach (Artist artist in artists)
            {
                var biography = string.Empty;
                if (artist.Biography != null && artist.Biography.Length > 60)
                {
                    biography = artist.Biography.Substring(0, 60);
                }

                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}, Biography: {biography}");
            }
        }

        public async Task GetArtists()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsAsync();

            foreach (Artist artist in artists)
            {
                var biography = string.Empty;
                if (artist.Biography != null && artist.Biography.Length > 60)
                {
                    biography = artist.Biography.Substring(0, 60);
                }

                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}, Biography: {biography}");
            }
        }

        public async Task GetArtistsWithNoBiographyAsync()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsWithNoBiographyAsync();

            foreach (Artist artist in artists)
            {
                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}");
            }
        }

        public async Task GetArtistWithNoBiographyAsync(string name)
        {
            Artist artist = await _artistRepository.GetArtistWithNoBiographyAsync(name);
            if (artist != null)
            {
                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}");
            }
            else
            {
                Console.WriteLine($"No artist found with the name '{name}' and no biography.");
            }
        }

        public async Task ShowArtistsAsync()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsAsync();

            foreach (Artist artist in artists)
            {
                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}");
            }
        }

        public async Task SelectAsync()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsAsync();

            foreach (Artist artist in artists)
            {
                var biography = string.Empty;
                if (artist.Biography != null && artist.Biography.Length > 60)
                {
                    biography = artist.Biography.Substring(0, 60);
                }

                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}, Biography: {biography}");
            }
        }

        public async Task GetArtistListAsync()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistListAsync();

            foreach (Artist artist in artists)
            {
                Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}");
            }
        }

        public async Task GetArtistNamesAsync()
        {
            IEnumerable<Artist> artists = await _artistRepository.GetArtistsAsync();

            foreach (Artist artist in artists)
            {
                Console.WriteLine($"Name: {artist.Name}");
            }
        }

        public async Task FindArtist(int artistId)
        {
            Artist artist = await _artistRepository.SelectAsync(artistId);

            Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}, Biography:\n\n{artist.Biography}");
        }

        public async Task GetSingleArtistAsync(int artistId)
        {
            Artist artist = await _artistRepository.SelectAsync(artistId);

            Console.WriteLine($"ArtistId: {artist.ArtistId}, Name: {artist.Name}, Biography:\n\n{artist.Biography}");
        }

        public async Task<int> CreateArtist()
        {
            var newArtist = new Artist
            {
                FirstName = "James",
                LastName = "Robson",
                Name = "James Robson",
                Biography = ""
            };

            int artistId = await _artistRepository.InsertAsync(newArtist);

            if (artistId is -1)
            {
                Console.WriteLine("Artist is already in the database!");
            }
            else
            {
                Console.WriteLine($"New Artist Id: {artistId}");
            }

            return artistId;
        }

        public async Task SelectArtistWithNoBioAsync()
        {
            var artists = await _artistRepository.SelectArtistWithNoBioAsync();

            foreach (var artist in artists)
            {
                Console.WriteLine($"{artist.ArtistId}: {artist.Name}");
            }
        }

        public async Task InsertAsync()
        {
            var artist = new Artist
            {
                FirstName = "Alan",
                LastName = "Robson",
                Biography = "<p>Alan is a electronic pioneer.</p>"
            };

            var artistId = await _artistRepository.InsertAsync(artist);

            Console.WriteLine(artistId);
        }

        public async Task Insert2Async()
        {
            var firstName = "Andrew";
            var lastName = "Robson";
            var biography = "Andrew likes Pocopunk.";

            var newArtistId = await _artistRepository.InsertAsync(firstName, lastName, biography);

            Console.WriteLine(newArtistId);
        }

        public async Task UpdateArtistAsync()
        {
            var artist = new Artist
            {
                ArtistId = 910,
                FirstName = "Alan",
                LastName = "Robsano",
                Name = "Alan Robsano",
                Biography = "Alan hates country and western. He hates both kinds of music."
            };

            var artistId = await _artistRepository.UpdateArtistAsync(artist);

            Console.WriteLine(artistId);
        }

        public async Task UpdateArtist2Async()
        {
            var artistId = 911;
            var firstName = "Chuck";
            var lastName = "Robson-Smith";
            var name = "Chuck Robson-Smith";
            var biography = "<p>Chuck is a superstar Pop singer.</p>";

            artistId = await _artistRepository.UpdateAsync(artistId, firstName, lastName, name, biography);

            Console.WriteLine(artistId);
        }

        public async Task GetArtistId2Async()
        {
            var recordId = 289;
            var artistId = await _artistRepository.GetArtistIdAsync(recordId);

            Console.WriteLine(artistId);
        }

        public async Task GetArtistIdAsync()
        {
            var artistId = await _artistRepository.GetArtistIdAsync("Bob", "Dylan");

            Console.WriteLine(artistId);
        }

        public async Task UpdateAsync()
        {
            var artist = new Artist
            {
                ArtistId = 910,
                FirstName = "Alan",
                LastName = "Robson",
                Name = "Alan Robson",
                Biography = "<p>Alan loves country and western. He loves both kinds of music.</p>"
            };

            var artistId = await _artistRepository.UpdateArtistAsync(artist);

            Console.WriteLine(artistId);
        }

        public async Task Update2Async()
        {
            var artistId = 912;
            var firstName = "James";
            var lastName = "Robson";
            var name = "James Robson";
            var biography = "<p>James plays a fast Polka dance music and has had success in Sweden and other Scandinavian countries.</p>";

            artistId = await _artistRepository.UpdateAsync(artistId, firstName, lastName, name, biography);

            Console.WriteLine(artistId);
        }

        public async Task ShowArtistAsync()
        {
            var artistId = 114;
            Artist artist = await _artistRepository.SelectAsync(artistId);

            Console.WriteLine(artist.ToString());
        }

        public async Task GetBiographyAsync()
        {
            var recordId = 283;
            var artistId = await _artistRepository.GetArtistIdAsync(recordId);
            var artist = await _artistRepository.SelectAsync(artistId);

            Console.WriteLine(artist.Biography);
        }

        public async Task DeleteArtistAsync()
        {
            var artistId = 914;
            await _artistRepository.DeleteAsync(artistId);

            Console.WriteLine("Artist deleted");
        }
    }
}
