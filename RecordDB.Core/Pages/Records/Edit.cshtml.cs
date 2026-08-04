using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class EditModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IRecordRepository recordRepository, ILogger<EditModel> logger)
        {
            _recordRepository = recordRepository;
            _logger = logger;
        }

        [BindProperty]
        public ArtistRecordDto Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Edit Record page for record {RecordId}", id);

            ArtistRecordDto record = await _recordRepository.SelectAsync(id);

            if (record is null)
            {
                _logger.LogWarning("Edit Record: record {RecordId} not found", id);
                return NotFound();
            }

            Record = record;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Record.Bought == default(DateTime))
            {
                Record.Bought = DateTime.Parse("1900-01-01");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit Record validation failed for record {RecordId}", Record.RecordId);
                return Page();
            }

            await _recordRepository.UpdateAsync(Record);
            _logger.LogInformation("Record '{RecordName}' (ID {RecordId}) updated successfully", Record.Name, Record.RecordId);

            return RedirectToPage("./Index");
        }
    }
}
