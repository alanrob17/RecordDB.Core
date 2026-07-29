using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class DeleteModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        public DeleteModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            await _recordRepository.DeleteAsync(Record.RecordId);
            return RedirectToPage("./Index");
        }
    }
}
