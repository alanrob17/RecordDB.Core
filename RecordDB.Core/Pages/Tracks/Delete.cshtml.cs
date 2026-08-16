using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Tracks
{
    public class DeleteModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(ITrackRepository trackRepository, ILogger<DeleteModel> logger)
        {
            _trackRepository = trackRepository;
            _logger = logger;
        }

        public ArtistRecordDiscTrackDto Track { get; set; } = default!;

        [BindProperty]
        public int TrackId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Delete Track confirmation page for track {TrackId}", id);

            var track = await _trackRepository.SelectTrackByIdAsync(id);

            if (track is null)
                return NotFound();

            Track  = track;
            TrackId = track.TrackId ?? 0;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _trackRepository.DeleteTrackAsync(TrackId);

            _logger.LogInformation("Track ID {TrackId} deleted successfully", TrackId);

            return RedirectToPage("./Index");
        }
    }
}
