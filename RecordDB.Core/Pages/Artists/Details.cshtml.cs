using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class DetailsModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IArtistRepository artistRepository, ILogger<DetailsModel> logger)
        {
            _artistRepository = artistRepository;
            _logger = logger;
        }

        public Artist Artist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Details page for artist {ArtistId}", id);

            var artist = await _artistRepository.SelectAsync(id);

            if (artist is null)
                return NotFound();

            Artist = artist;
            return Page();
        }
    }
}
