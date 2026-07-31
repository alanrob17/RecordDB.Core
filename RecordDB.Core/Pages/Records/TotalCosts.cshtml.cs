using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class TotalCostsModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        private const int PageSize = 20;

        public TotalCostsModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public IList<Total> Totals { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            List<Total> allTotals = await _recordRepository.GetTotalCostsAsync();

            TotalCount = allTotals.Count;
            CurrentPage = Math.Max(1, pageNumber);
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            Totals = allTotals
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
