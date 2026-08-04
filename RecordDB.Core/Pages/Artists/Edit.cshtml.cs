using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class EditModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IArtistRepository artistRepository, ILogger<EditModel> logger)
        {
            _artistRepository = artistRepository;
            _logger = logger;
        }

        [BindProperty]
        public Artist Artist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Edit Artist page for artist {ArtistId}", id);

            var artist = await _artistRepository.SelectAsync(id);

            if (artist is null)
                return NotFound();

            Artist = artist;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit Artist validation failed for artist {ArtistId}", Artist.ArtistId);
                return Page();
            }

            await _artistRepository.UpdateArtistAsync(Artist);
            _logger.LogInformation("Artist '{ArtistName}' (ID {ArtistId}) updated successfully", Artist.Name, Artist.ArtistId);

            return RedirectToPage("./Index");
        }
    }
}
