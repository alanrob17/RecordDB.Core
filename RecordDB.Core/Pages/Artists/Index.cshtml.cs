using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class IndexModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;

        private const int PageSize = 20;

        public IndexModel(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public IList<Artist> Artists { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            var all = (await _artistRepository.GetArtistsAsync()).ToList();

            TotalCount = all.Count;
            CurrentPage = Math.Max(1, pageNumber);
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            Artists = all
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
