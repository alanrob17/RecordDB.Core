using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RecordDB.Core.Pages.Tracks
{
    public class SearchModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;

        public SearchModel(ITrackRepository trackRepository)
        {
            _trackRepository = trackRepository;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter a track name to search.")]
        [Display(Name = "Track Name")]
        public string TrackName { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string trackName = TrackName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(trackName))
            {
                ModelState.AddModelError(nameof(TrackName), "Please enter a track name.");
                return Page();
            }

            // Redirect to Pages/Tracks/Index with trackName as query parameter
            return RedirectToPage("/Tracks/Index", new { trackName = trackName });
        }
    }
}
