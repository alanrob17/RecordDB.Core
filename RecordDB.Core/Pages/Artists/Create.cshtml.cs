using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class CreateModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;

        public CreateModel(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        [BindProperty]
        public Artist Artist { get; set; } = new Artist();

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

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
