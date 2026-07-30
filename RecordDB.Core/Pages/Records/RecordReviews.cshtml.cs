using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class RecordReviewsModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;
        private const int PageSize = 20;

        public RecordReviewsModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public IList<RecordReviewDto> Reviews { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            var all = await _recordRepository.SelectRecordReviewsAsync();

            TotalCount = all.Count;
            CurrentPage = Math.Max(1, pageNumber);
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            Reviews = all
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
