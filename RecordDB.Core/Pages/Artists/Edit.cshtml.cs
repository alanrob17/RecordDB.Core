using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class EditModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;

        public EditModel(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        [BindProperty]
        public Artist Artist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var artist = await _artistRepository.SelectAsync(id);

            if (artist is null)
                return NotFound();

            Artist = artist;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _artistRepository.UpdateArtistAsync(Artist);

            return RedirectToPage("./Index");
        }
    }
}
