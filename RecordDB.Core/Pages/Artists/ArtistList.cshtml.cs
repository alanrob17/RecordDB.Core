using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class ArtistListModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IRecordRepository _recordRepository;

        public ArtistListModel(IArtistRepository artistRepository, IRecordRepository recordRepository)
        {
            _artistRepository = artistRepository;
            _recordRepository = recordRepository;
        }

        public class ArtistGroup
        {
            public Artist Artist { get; set; } = default!;
            public List<ArtistRecordDto> Records { get; set; } = [];
        }

        public List<ArtistGroup> ArtistGroups { get; set; } = [];

        public async Task OnGetAsync()
        {
            var artists = await _artistRepository.SelectAsync();
            var records = await _recordRepository.SelectAsync();

            var recordsByArtist = records
                .GroupBy(r => r.ArtistId)
                .ToDictionary(g => g.Key, g => g.ToList());

            ArtistGroups = artists.Select(artist => new ArtistGroup
            {
                Artist = artist,
                Records = recordsByArtist.TryGetValue(artist.ArtistId, out var artistRecords)
                    ? artistRecords
                    : []
            }).ToList();
        }
    }
}
