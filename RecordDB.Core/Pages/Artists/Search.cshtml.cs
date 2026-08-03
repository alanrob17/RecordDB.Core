using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RecordDB.Core.Pages.Artists
{
    public class SearchModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;

        public SearchModel(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        [BindProperty]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string firstName = FirstName?.Trim() ?? string.Empty;
            string lastName = LastName?.Trim() ?? string.Empty;

            int artistId = await _artistRepository.GetArtistIdAsync(firstName, lastName);

            if (artistId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Artist not found. Please check spelling and try again.");
                return Page();
            }

            // Run SelectAsync to retrieve the artist (using up_ArtistSelectById)
            var artist = await _artistRepository.SelectAsync(artistId);
            if (artist == null)
            {
                ModelState.AddModelError(string.Empty, "Artist details could not be retrieved.");
                return Page();
            }

            return RedirectToPage("/Records/GetRecords", new { show = $"aid{artistId}" });
        }
    }
}
