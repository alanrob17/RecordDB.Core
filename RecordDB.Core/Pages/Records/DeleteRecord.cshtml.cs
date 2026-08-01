using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class DeleteRecordModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IArtistRepository _artistRepository;

        public DeleteRecordModel(IRecordRepository recordRepository, IArtistRepository artistRepository)
        {
            _recordRepository = recordRepository;
            _artistRepository = artistRepository;
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
                var record = await _recordRepository.SelectAsync(targetRecordId);
                if (record != null)
                {
                    Record = record;
                    SelectedRecordId = record.RecordId;
                    SelectedArtistId = record.ArtistId;
                }
            }

            await PopulateDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Record != null && Record.RecordId > 0)
            {
                await _recordRepository.DeleteAsync(Record.RecordId);
            }

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
