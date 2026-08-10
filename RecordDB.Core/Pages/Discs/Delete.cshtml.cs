using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Threading.Tasks;

namespace RecordDB.Core.Pages.Discs
{
    public class DeleteModel : PageModel
    {
        private readonly IDiscRepository _discRepository;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IDiscRepository discRepository, ILogger<DeleteModel> logger)
        {
            _discRepository = discRepository;
            _logger = logger;
        }

        [BindProperty]
        public ArtistRecordDiscDto Disc { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Delete Disc confirmation page for disc {DiscId}", id);

            var disc = await _discRepository.SelectSingleDiscAsync(id);

            if (disc is null)
                return NotFound();

            Disc = disc;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _discRepository.DeleteDiscAsync(Disc.DiscId);

            _logger.LogInformation("Disc ID {DiscId} deleted successfully", Disc.DiscId);

            return RedirectToPage("./Index");
        }
    }
}
