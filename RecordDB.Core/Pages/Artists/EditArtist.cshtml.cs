using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.Core.Pages.Records;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class EditArtistModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<EditArtistModel> _logger;

        public EditArtistModel(IArtistRepository artistRepository, ILogger<EditArtistModel> logger)
        {
            _artistRepository = artistRepository;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int? SelectedArtistId { get; set; }

        [BindProperty]
        public Artist Artist { get; set; } = new();

        public SelectList ArtistList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int targetArtistId = id ?? SelectedArtistId ?? 0;
            if (targetArtistId > 0)
            {
                SelectedArtistId = targetArtistId;

                _logger.LogDebug("EditArtist: loading artist {ArtistId}", targetArtistId);
                var artist = await _artistRepository.SelectAsync(targetArtistId);
                if (artist != null)
                {
                    Artist = artist;
                }
            }

            await PopulateArtistListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("EditArtist validation failed for artist {ArtistId}", Artist.ArtistId);
                await PopulateArtistListAsync();
                return Page();
            }

            await _artistRepository.UpdateArtistAsync(Artist);
            _logger.LogInformation("Artist '{ArtistName}' (ID {ArtistId}) updated successfully", Artist.Name,Artist.ArtistId);

            return RedirectToPage("./Index");
        }

        private async Task PopulateArtistListAsync()
        {
            var artists = await _artistRepository.GetArtistListAsync();
            ArtistList = new SelectList(artists, nameof(Artist.ArtistId), nameof(Artist.Name), SelectedArtistId);
        }
    }
}
