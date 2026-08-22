using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class NoBiographyModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;

        private const int PageSize = 20;

        public NoBiographyModel(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public IList<Artist> Artists { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        /// <summary>True when the list is filtered by a search term.</summary>
        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchTerm);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            if (IsSearching)
            {
                // Search results — call the dedicated no-biography search sproc
                var match = await _artistRepository.GetArtistWithNoBiographyAsync(SearchTerm!);

                Artists     = match is not null ? [match] : [];
                TotalCount  = Artists.Count;
                CurrentPage = 1;
                TotalPages  = 1;
            }
            else
            {
                // Full list with pagination
                var all = (await _artistRepository.GetArtistsWithNoBiographyAsync()).ToList();

                TotalCount  = all.Count;
                CurrentPage = Math.Max(1, pageNumber);
                TotalPages  = (int)Math.Ceiling(TotalCount / (double)PageSize);
                CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

                Artists = all
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }
    }
}
