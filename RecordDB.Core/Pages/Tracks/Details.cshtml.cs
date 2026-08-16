using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Tracks
{
    public class DetailsModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(ITrackRepository trackRepository, ILogger<DetailsModel> logger)
        {
            _trackRepository = trackRepository;
            _logger = logger;
        }

        public ArtistRecordDiscTrackDto Track { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Details page for track {TrackId}", id);

            var track = await _trackRepository.SelectTrackByIdAsync(id);

            if (track is null)
                return NotFound();

            Track = track;
            return Page();
        }
    }
}
