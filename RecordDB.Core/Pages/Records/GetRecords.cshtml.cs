using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class GetRecordsModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        private const int PageSize = 20;

        /// <summary>Maps the route segment (e.g. "cd", "rock", "2022") to a human-readable page heading.</summary>
        private static readonly Dictionary<string, string> HeaderMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "all",           "All Records and CD's" },
            { "cd",            "All CD's" },
            { "records",       "All Records" },
            { "dvds",          "All DVD's" },
            { "blurays",       "All Blurays" },
            { "2022",          "All Records bought in 2022" },
            { "2021",          "All Records bought in 2021" },
            { "2020",          "All Records bought in 2020" },
            { "2019",          "All Records bought in 2019" },
            { "2018",          "All Records bought in 2018" },
            { "2017",          "All Records bought in 2017" },
            { "1111",          "Indispensible Records" },
            { "Rock",          "Rock Albums" },
            { "Blues",         "Blues Albums" },
            { "Jazz",          "Jazz Albums" },
            { "Classical",     "Classical Albums" },
            { "Soundtrack",    "Soundtrack Albums" },
            { "Country",       "Country Albums" },
            { "Rockdesc",      "Rock Albums by date" },
            { "Bluesdesc",     "Blues Albums by date" },
            { "Jazzdesc",      "Jazz Albums by date" },
            { "Classicaldesc", "Classical Albums by date" },
            { "Soundtrackdesc","Soundtrack Albums by date" },
            { "Countrydesc",   "Country Albums by date" },
        };

        public GetRecordsModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public IList<ArtistRecordDto> Records { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        /// <summary>The route segment passed in the URL (e.g. "cd", "Rock", "2022").</summary>
        [BindProperty(SupportsGet = true)]
        public string Show { get; set; } = "all";

        /// <summary>Human-readable heading derived from the Show route value.</summary>
        public string PageHeading { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            // Resolve heading — check for rdddd format (Recorded Year search) first, otherwise fall back to HeaderMap
            if (Show != null && Show.Length == 5 && (Show[0] == 'r' || Show[0] == 'R') && int.TryParse(Show.Substring(1), out int year))
            {
                PageHeading = $"All Albums recorded in {year}";
            }
            else
            {
                PageHeading = HeaderMap.TryGetValue(Show ?? "all", out var heading)
                    ? heading
                    : Show ?? "All Records";
            }

            var all = await _recordRepository.SelectRecordsShowAsync(Show);

            TotalCount  = all.Count;
            CurrentPage = Math.Max(1, pageNumber);
            TotalPages  = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            Records = all
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }
    }
}
