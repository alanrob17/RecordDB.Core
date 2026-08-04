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
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IRecordRepository recordRepository, ILogger<DeleteModel> logger)
        {
            _recordRepository = recordRepository;
            _logger = logger;
        }

        [BindProperty]
        public ArtistRecordDto Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Delete Record confirmation page for record {RecordId}", id);

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
            _logger.LogInformation("Record '{RecordName}' (ID {RecordId}) deleted successfully", Record.Name, Record.RecordId);

            return RedirectToPage("./Index");
        }
    }
}
