using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class DeleteModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IArtistRepository artistRepository, ILogger<DeleteModel> logger)
        {
            _artistRepository = artistRepository;
            _logger = logger;
        }

        [BindProperty]
        public Artist Artist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Delete Record confirmation page for artist {ArtistId}", id);

            var artist = await _artistRepository.SelectAsync(id);

            if (artist is null)
                return NotFound();

            Artist = artist;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _artistRepository.DeleteAsync(Artist.ArtistId);

            _logger.LogInformation("Artist '{ArtistName}' (ID {ArtistId}) deleted successfully", Artist.Name, Artist.ArtistId);

            return RedirectToPage("./Index");
        }
    }
}
