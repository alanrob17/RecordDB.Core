using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class RecordViewModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        public RecordViewModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public ArtistRecordDto Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ArtistRecordDto record = await _recordRepository.SelectAsync(id);

            if (record is null)
            {
                return NotFound();
            }

            Record = record;
            return Page();
        }
    }
}
