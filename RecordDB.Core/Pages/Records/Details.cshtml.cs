using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class DetailsModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IRecordRepository recordRepository, ILogger<DetailsModel> logger)
        {
            _recordRepository = recordRepository;
            _logger = logger;
        }

        public ArtistRecordDto Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Record Details for record {RecordId}", id);

            ArtistRecordDto record = await _recordRepository.SelectAsync(id);

            if (record is null)
            {
                _logger.LogWarning("Record Details: record {RecordId} not found", id);
                return NotFound();
            }

            Record = record;

            return Page();
        }
    }
}
