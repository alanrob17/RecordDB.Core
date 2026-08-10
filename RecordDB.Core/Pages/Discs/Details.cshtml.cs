using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Threading.Tasks;

namespace RecordDB.Core.Pages.Discs
{
    public class DetailsModel : PageModel
    {
        private readonly IDiscRepository _discRepository;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IDiscRepository discRepository, ILogger<DetailsModel> logger)
        {
            _discRepository = discRepository;
            _logger = logger;
        }

        public ArtistRecordDiscDto Disc { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Loading Details page for disc {DiscId}", id);

            var disc = await _discRepository.SelectSingleDiscAsync(id);

            if (disc is null)
                return NotFound();

            Disc = disc;
            return Page();
        }
    }
}
