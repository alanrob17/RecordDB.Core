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

        /// <summary>True when the list is filtered by artist name or year search.</summary>
        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchTerm);

        /// <summary>True when the search term is a 4-digit recorded year.</summary>
        public bool IsYearSearch => IsSearching && SearchTerm!.Trim().Length == 4 && int.TryParse(SearchTerm.Trim(), out _);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            List<ArtistRecordDto> all;

            if (IsSearching)
            {
                var trimmedSearch = SearchTerm!.Trim();
                if (trimmedSearch.Length == 4 && int.TryParse(trimmedSearch, out int recordedYear))
                {
                    all = await _recordRepository.GetRecordsByYearAsync(recordedYear);
                }
                else
                {
                    all = await _recordRepository.GetRecordsByArtistNameAsync(SearchTerm!);
                }
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
