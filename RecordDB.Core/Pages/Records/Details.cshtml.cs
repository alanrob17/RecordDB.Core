using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class DetailsModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        public DetailsModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public Record Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var record = await _recordRepository.SelectAsync(id);

            if (record is null)
                return NotFound();

            Record = record;
            return Page();
        }
    }
}
