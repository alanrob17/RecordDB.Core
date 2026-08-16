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

        /// <summary>True when the list is filtered by a partial record name.</summary>
        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchTerm);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            List<ArtistRecordDiscTrackDto> all;

            if (IsSearching)
            {
                // Search results by partial record name — no pagination
                all = (await _trackRepository.SelectArtistRecordTracksAsync(SearchTerm!)).ToList();
                TotalCount  = all.Count;
                CurrentPage = 1;
                TotalPages  = 1;
                Tracks      = all;
            }
            else
            {
                // Full list with pagination
                all = (await _trackRepository.SelectAllTrackEntitiesAsync()).ToList();

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
}
