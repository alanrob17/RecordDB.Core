using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.Core.Pages.Records;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class CreateModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IArtistRepository artistRepository, ILogger<CreateModel> logger)
        {
            _artistRepository = artistRepository;
            _logger = logger;
        }

        [BindProperty]
        public Artist Artist { get; set; } = new Artist();

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _logger.LogInformation("Inserting new artist '{ArtistName}'", Artist.Name);

            var newId = await _artistRepository.InsertAsync(Artist);

            if (newId <= 0)
            {
                // 0 = artist already exists in the database
                ModelState.AddModelError(string.Empty,
                    newId == 0
                        ? "This artist already exists in the database."
                        : "An error occurred while saving. Please try again.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
