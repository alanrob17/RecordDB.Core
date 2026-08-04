using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class EditRecordModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<EditRecordModel> _logger;

        public EditRecordModel(IRecordRepository recordRepository, IArtistRepository artistRepository, ILogger<EditRecordModel> logger)
        {
            _recordRepository = recordRepository;
            _artistRepository = artistRepository;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int? SelectedArtistId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedRecordId { get; set; }

        [BindProperty]
        public ArtistRecordDto Record { get; set; } = new();

        public SelectList ArtistList { get; set; } = default!;
        public SelectList RecordList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int targetRecordId = id ?? SelectedRecordId ?? 0;

            if (targetRecordId > 0)
            {
                _logger.LogDebug("EditRecord: loading record {RecordId}", targetRecordId);
                var record = await _recordRepository.SelectAsync(targetRecordId);
                if (record != null)
                {
                    Record = record;
                    SelectedRecordId = record.RecordId;
                    SelectedArtistId = record.ArtistId;
                }
                else
                {
                    _logger.LogWarning("EditRecord: record {RecordId} not found", targetRecordId);
                }
            }
            else if (SelectedArtistId.HasValue)
            {
                _logger.LogDebug("EditRecord: artist {ArtistId} selected, awaiting record selection", SelectedArtistId);
            }

            await PopulateDropdownsAsync();
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
                _logger.LogWarning("EditRecord validation failed for record {RecordId}", Record.RecordId);
                await PopulateDropdownsAsync();
                return Page();
            }

            await _recordRepository.UpdateAsync(Record);
            _logger.LogInformation("Record '{RecordName}' (ID {RecordId}) updated successfully", Record.Name, Record.RecordId);

            return RedirectToPage("./Index");
        }

        private async Task PopulateDropdownsAsync()
        {
            var artists = await _artistRepository.GetArtistListAsync();
            ArtistList = new SelectList(artists, nameof(Artist.ArtistId), nameof(Artist.Name), SelectedArtistId);

            if (SelectedArtistId.HasValue && SelectedArtistId.Value > 0)
            {
                var records = await _recordRepository.SelectArtistRecordsAsync(SelectedArtistId.Value);
                RecordList = new SelectList(records, nameof(RecordDB.DAL.Models.Record.RecordId), nameof(RecordDB.DAL.Models.Record.Name), SelectedRecordId);
            }
            else
            {
                RecordList = new SelectList(Enumerable.Empty<RecordDB.DAL.Models.Record>(), nameof(RecordDB.DAL.Models.Record.RecordId), nameof(RecordDB.DAL.Models.Record.Name));
            }
        }
    }
}
