using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Tracks
{
    public class CreateModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IDiscRepository _discRepository;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ITrackRepository trackRepository, IDiscRepository discRepository, ILogger<CreateModel> logger)
        {
            _trackRepository = trackRepository;
            _discRepository = discRepository;
            _logger = logger;
        }

        [BindProperty]
        public Track Track { get; set; } = new Track();

        public IActionResult OnGet() => Page();

        /// <summary>
        /// JSON handler used by the cascading dropdowns on the Create form.
        /// Returns all disc entries whose record name contains <paramref name="name"/>.
        /// </summary>
        public async Task<IActionResult> OnGetSearchAsync(string name)
        {
            var discs = await _discRepository.GetDiscRecordsByRecordNameAsync(name ?? string.Empty);
            var result = discs.Select(d => new
            {
                d.RecordId,
                d.DiscId,
                d.ArtistName,
                d.Name,
                d.DiscNo
            });
            return new JsonResult(result);
        }

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
