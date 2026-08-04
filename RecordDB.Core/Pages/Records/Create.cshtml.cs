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
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IRecordRepository recordRepository, IArtistRepository artistRepository, ILogger<CreateModel> logger)
        {
            _recordRepository = recordRepository;
            _artistRepository = artistRepository;
            _logger = logger;
        }

        [BindProperty]
        public Record Record { get; set; } = new Record { Bought = DateTime.Today, Recorded = DateTime.Today.Year };

        public SelectList ArtistList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogDebug("Loading Add Record page");
            await PopulateArtistListAsync();
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
                await PopulateArtistListAsync();
                return Page();
            }

            _logger.LogInformation("Inserting new record '{RecordName}' for artist {ArtistId}", Record.Name, Record.ArtistId);

            var newId = await _recordRepository.InsertAsync(Record);

            if (newId <= 0)
            {
                if (newId == 0)
                {
                    ModelState.AddModelError(string.Empty, "This record already exists in the database.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving. Please try again.");
                }

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
