using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Tracks
{
    public class EditModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ITrackRepository trackRepository, ILogger<EditModel> logger)
        {
            _trackRepository = trackRepository;
            _logger = logger;
        }

        [BindProperty]
        public Track Track { get; set; } = default!;

        /// <summary>Read-only context displayed on the form.</summary>
        public string? ArtistName { get; set; }
        public string? RecordName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Edit Track page for track {TrackId}", id);

            var dto = await _trackRepository.SelectTrackByIdAsync(id);

            if (dto is null)
                return NotFound();

            ArtistName = dto.ArtistName;
            RecordName = dto.Name;

            Track = new Track
            {
                TrackId     = dto.TrackId ?? 0,
                DiscId      = dto.DiscId,
                TrackNo     = dto.TrackNo ?? 0,
                Name        = dto.TrackName,
                TrackLength = dto.TrackLength,
                Extended    = dto.Extended
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit Track validation failed for track {TrackId}", Track.TrackId);
                return Page();
            }

            await _trackRepository.UpdateTrackAsync(Track);
            _logger.LogInformation("Track ID {TrackId} updated successfully", Track.TrackId);

            return RedirectToPage("./Index");
        }
    }
}
