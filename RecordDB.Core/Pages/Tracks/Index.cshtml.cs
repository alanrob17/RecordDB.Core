using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Tracks
{
    public class IndexModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;

        private const int PageSize = 20;

        public IndexModel(ITrackRepository trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public IList<ArtistRecordDiscTrackDto> Tracks { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        /// <summary>True when the list is filtered by a partial record name or track name.</summary>
        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchTerm) || !string.IsNullOrWhiteSpace(TrackName);

        /// <summary>True when searching specifically by track name.</summary>
        public bool IsTrackSearch => !string.IsNullOrWhiteSpace(TrackName);

        /// <summary>Search term for record name search.</summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        /// <summary>Search term for track name search.</summary>
        [BindProperty(SupportsGet = true)]
        public string? TrackName { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            List<ArtistRecordDiscTrackDto> all;

            if (IsTrackSearch)
            {
                // Search results by partial track name
                all = (await _trackRepository.SelectTracksByPartialNameAsync(TrackName!.Trim())).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Search results by partial record name
                all = (await _trackRepository.SelectArtistRecordTracksAsync(SearchTerm!.Trim())).ToList();
            }
            else
            {
                // Full list
                all = (await _trackRepository.SelectAllTrackEntitiesAsync()).ToList();
            }

            TotalCount  = all.Count;
            CurrentPage = Math.Max(1, pageNumber);
            TotalPages  = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            Tracks = all
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
