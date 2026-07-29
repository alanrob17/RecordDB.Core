using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class CreateModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IArtistRepository _artistRepository;

        public CreateModel(IRecordRepository recordRepository, IArtistRepository artistRepository)
        {
            _recordRepository = recordRepository;
            _artistRepository = artistRepository;
        }

        [BindProperty]
        public Record Record { get; set; } = new Record { Bought = DateTime.Today, Recorded = DateTime.Today.Year };

        public SelectList ArtistList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateArtistListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Record.Bought");
            ModelState.Remove("Record.CoverName");

            if (!ModelState.IsValid)
            {
                await PopulateArtistListAsync();
                return Page();
            }

            var newId = await _recordRepository.InsertAsync(Record);

            if (newId <= 0)
            {
                ModelState.AddModelError(string.Empty,
                    newId == 0
                        ? "This record already exists in the database."
                        : "An error occurred while saving. Please try again.");
                await PopulateArtistListAsync();
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task PopulateArtistListAsync()
        {
            var artists = await _artistRepository.GetArtistListAsync();
            ArtistList = new SelectList(artists, nameof(Artist.ArtistId), nameof(Artist.Name));
        }
    }
}
