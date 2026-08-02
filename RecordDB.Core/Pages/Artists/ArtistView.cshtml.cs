using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class ArtistViewModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistViewModel(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public Artist Artist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Artist artist = await _artistRepository.SelectAsync(id);

            if (artist is null)
            {
                return NotFound();
            }

            Artist = artist;
            return Page();
        }
    }
}
