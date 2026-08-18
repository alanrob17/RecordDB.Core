using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class RecordViewModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly ITrackRepository _trackRepository;

        public RecordViewModel(IRecordRepository recordRepository, ITrackRepository trackRepository)
        {
            _recordRepository = recordRepository;
            _trackRepository = trackRepository;
        }

        public ArtistRecordDto Record { get; set; } = default!;

        /// <summary>
        /// All raw track rows for this record, ordered by disc then track number.
        /// Each row also carries the disc-level fields (DiscId, DiscNo, Length).
        /// Fetched via up_GetArtistRecordTracks by record name.
        /// </summary>
        public IReadOnlyList<ArtistRecordDiscTrackDto> AllTracks { get; set; }
            = Array.Empty<ArtistRecordDiscTrackDto>();

        /// <summary>
        /// Track rows that have a TrackId, grouped by DiscNo.
        /// </summary>
        public IReadOnlyDictionary<int, IReadOnlyList<ArtistRecordDiscTrackDto>> TracksByDisc { get; set; }
            = new Dictionary<int, IReadOnlyList<ArtistRecordDiscTrackDto>>();

        /// <summary>
        /// One representative row per disc (carries disc-level metadata such as Length).
        /// </summary>
        public IReadOnlyList<ArtistRecordDiscTrackDto> Discs { get; set; }
            = Array.Empty<ArtistRecordDiscTrackDto>();

        public bool HasTracks => TracksByDisc.Count > 0;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ArtistRecordDto record = await _recordRepository.SelectAsync(id);

            if (record is null)
            {
                return NotFound();
            }

            Record = record;

            // Load track listing by record name using up_GetArtistRecordTracks
            if (!string.IsNullOrWhiteSpace(record.Name))
            {
                var raw = (await _trackRepository.SelectTracksByRecordAsync(record.Name))
                    .OrderBy(t => t.DiscNo)
                    .ThenBy(t => t.TrackNo)
                    .ToList();

                AllTracks = raw;

                // Group actual tracks by disc number
                TracksByDisc = raw
                    .Where(t => t.TrackId.HasValue)
                    .GroupBy(t => t.DiscNo)
                    .ToDictionary(
                        g => g.Key,
                        g => (IReadOnlyList<ArtistRecordDiscTrackDto>)g.ToList());

                // One disc-metadata row per disc (first row per disc carries disc Length etc.)
                Discs = raw
                    .GroupBy(t => t.DiscNo)
                    .Select(g => g.First())
                    .OrderBy(d => d.DiscNo)
                    .ToList();
            }

            return Page();
        }

        /// <summary>Formats seconds to m:ss (or h:mm:ss for recordings over 1 hour).</summary>
        public static string FormatLength(int? totalSeconds)
        {
            if (!totalSeconds.HasValue || totalSeconds.Value <= 0) return string.Empty;
            var ts = TimeSpan.FromSeconds(totalSeconds.Value);
            return ts.TotalHours >= 1
                ? ts.ToString(@"h\:mm\:ss")
                : ts.ToString(@"m\:ss");
        }
    }
}
