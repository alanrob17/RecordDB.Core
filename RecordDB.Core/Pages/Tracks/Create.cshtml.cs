using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Tracks
{
    public class CreateModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ITrackRepository trackRepository, ILogger<CreateModel> logger)
        {
            _trackRepository = trackRepository;
            _logger = logger;
        }

        [BindProperty]
        public Track Track { get; set; } = new Track();

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _logger.LogInformation("Inserting new track '{TrackName}' for DiscId {DiscId}", Track.Name, Track.DiscId);

            var newId = await _trackRepository.InsertTrackAsync(Track);

            if (newId <= 0)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the track. Please try again.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
