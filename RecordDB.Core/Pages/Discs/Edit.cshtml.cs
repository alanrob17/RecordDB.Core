using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Threading.Tasks;

namespace RecordDB.Core.Pages.Discs
{
    public class EditModel : PageModel
    {
        private readonly IDiscRepository _discRepository;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IDiscRepository discRepository, ILogger<EditModel> logger)
        {
            _discRepository = discRepository;
            _logger = logger;
        }

        [BindProperty]
        public ArtistRecordDiscDto Disc { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Edit Disc page for disc {DiscId}", id);

            var disc = await _discRepository.SelectSingleDiscAsync(id);

            if (disc is null)
                return NotFound();

            Disc = disc;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit Disc validation failed for disc {DiscId}", Disc?.DiscId);
                return Page();
            }

            var discEntity = CreateUpdateDisc(Disc);

            await _discRepository.UpdateDiscAsync(discEntity);
            _logger.LogInformation("Disc ID {DiscId} updated successfully", Disc.DiscId);

            return RedirectToPage("./Index");
        }
        private Disc CreateUpdateDisc(ArtistRecordDiscDto dto)
        {
            return new Disc
            {
                DiscId = dto.DiscId,
                RecordId = dto.RecordId,
                DiscNo = dto.DiscNo,
                FreeDbDiscId = dto.FreeDbDiscId,
                FreeDbId = dto.FreeDbId,
                Length = dto.Length
            };
        }
    }
}
