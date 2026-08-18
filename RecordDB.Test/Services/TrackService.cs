using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecordDB.Test.Services
{
    public class TrackService : ITrackService
    {
        private readonly ITrackRepository _trackRepository;

        public TrackService(ITrackRepository trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task RunAsync()
        {
            // await SelectTrack();
            // await SelectTracksAsync();
            // await SelectArtistRecordTracksAsync();
            // await SelectTracksByRecordAsync();
            // await GetNumberOfTracksAsync();
            // await InsertTrackAsync();
            // await UpdateTrackAsync();
            // await DeleteTrackAsync();
            await BulkInsertAsync();
        }

        public async Task SelectTrack()
        {
            var track = await _trackRepository.SelectTrackByIdAsync(5000);
            if (track != null)
            {
                string length = track.TrackLength.HasValue ? TimeSpan.FromSeconds(track.TrackLength.Value).ToString(@"hh\:mm\:ss") : "unk";
                Console.WriteLine($"{track.ArtistName}: {track.Name} - {track.DiscNo} - {track.TrackNo} - {track.TrackName} - {length}");
            }
            else
            {
                Console.WriteLine("Track not found.");
            }
        }

        public async Task SelectTracksAsync()
        {
            var tracks = await _trackRepository.SelectAllTrackEntitiesAsync();
            foreach (var track in tracks)
            {
                string length = track.Length.HasValue ? TimeSpan.FromSeconds(track.Length.Value).ToString(@"hh\:mm\:ss") : "unk";
                Console.WriteLine($"{track.ArtistName}: {track.Name} - {track.DiscNo} - {track.TrackNo} - {track.Name} - {length}");
            }
        }

        public async Task SelectTracksByRecordAsync()
        {
            var name = "John Wesley Harding";
            var tracks = await _trackRepository.SelectTracksByRecordAsync(name);
            foreach (var track in tracks)
            {
                string length = track.TrackLength.HasValue ? TimeSpan.FromSeconds(track.TrackLength.Value).ToString(@"hh\:mm\:ss") : "unk";
                Console.WriteLine($"{track.ArtistName}: {track.Name} - {track.DiscNo} - {track.TrackNo} - {track.TrackName} - {length}");
            }
        }

        public async Task SelectArtistRecordTracksAsync()
        {
            var name = "Blonde On Blonde";

            var tracks = await _trackRepository.SelectArtistRecordTracksAsync(name.ToString());
            foreach (var track in tracks)
            {
                string length = track.TrackLength.HasValue ? TimeSpan.FromSeconds(track.TrackLength.Value).ToString(@"hh\:mm\:ss") : "unk";
                Console.WriteLine($"{track.ArtistName}: {track.Name} - {track.DiscNo} - {track.TrackNo} - {track.TrackName} - {length}");
            }
        }

        public async Task GetNumberOfTracksAsync()
        {
            var recordId = 290;
            var trackNumber = await _trackRepository.GetTrackNumberAsync(recordId);
            Console.WriteLine($"Number of tracks for record {recordId}: {trackNumber}");
        }

        public async Task InsertTrackAsync()
        {
            var track = new Track
            {
                DiscId = 5609,
                TrackNo = 1,
                Name = "Wobble With The Wibble Dogs",
                TrackLength = 180,
                Extended = "Test Extended"
            };

            var trackId = await _trackRepository.InsertTrackAsync(track);
            Console.WriteLine($"Inserted track with ID: {trackId}");
        }

        public async Task UpdateTrackAsync()
        {
            var trackId = 9817;

            var track = new Track
            {
                TrackId = trackId,
                TrackNo = 1,
                Name = "Wobbling With The Wibble Dogs",
                TrackLength = 240,
                Extended = "Test Updated"
            };

            trackId = await _trackRepository.UpdateTrackAsync(track);

            if (trackId is -1)
            {
                Console.WriteLine("Track does not exist!");
            }
            else
            {
                Console.WriteLine($"Updated Track Id: {trackId}");
            }
        }

        public async Task DeleteTrackAsync()
        {
            var trackId = 9817;

            await _trackRepository.DeleteTrackAsync(trackId);
        }

        public async Task BulkInsertAsync()
        {
            List<Track> tracks = new List<Track>
            {
                new Track
                {
                    DiscId = 5606,
                    TrackNo = 1,
                    Name = "Wobble With The Wibble Dogs",
                    TrackLength = 180,
                    Extended = "Test Extended"
                },
                new Track
                {
                    DiscId = 5606,
                    TrackNo = 2,
                    Name = "Another Track",
                    TrackLength = 200,
                    Extended = "Test Extended 2"
                }
            };

            await _trackRepository.BulkTrackInsertAsync(tracks);
        }
    }
}