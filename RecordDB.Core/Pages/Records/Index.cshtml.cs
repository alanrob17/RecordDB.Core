using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class IndexModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        private const int PageSize = 20;

        public IndexModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public IList<ArtistRecordDto> Records { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        /// <summary>True when the list is filtered by an artist name search.</summary>
        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchTerm);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            List<ArtistRecordDto> all;

            if (IsSearching)
            {
                all = await _recordRepository.GetRecordsByArtistNameAsync(SearchTerm!);
            }
            else
            {
                all = await _recordRepository.SelectAsync();
            }

            TotalCount  = all.Count;
            CurrentPage = Math.Max(1, pageNumber);
            TotalPages  = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            Records = all
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
