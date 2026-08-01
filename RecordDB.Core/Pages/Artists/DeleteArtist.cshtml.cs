using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Artists
{
    public class DeleteArtistModel : PageModel
    {
        private readonly IArtistRepository _artistRepository;

        public DeleteArtistModel(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
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
            if (Artist != null && Artist.ArtistId > 0)
            {
                await _artistRepository.DeleteAsync(Artist.ArtistId);
            }

            return RedirectToPage("./Index");
        }

        private async Task PopulateArtistListAsync()
        {
            var artists = await _artistRepository.GetArtistListAsync();
            ArtistList = new SelectList(artists, nameof(Artist.ArtistId), nameof(Artist.Name), SelectedArtistId);
        }
    }
}
