using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Threading.Tasks;

namespace RecordDB.Core.Pages.Discs
{
    public class CreateModel : PageModel
    {
        private readonly IDiscRepository _discRepository;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IDiscRepository discRepository, ILogger<CreateModel> logger)
        {
            _discRepository = discRepository;
            _logger = logger;
        }

        [BindProperty]
        public Disc Disc { get; set; } = new Disc();

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _logger.LogInformation("Inserting new disc for RecordId {RecordId}, DiscNo {DiscNo}", Disc.RecordId, Disc.DiscNo);

            var newId = await _discRepository.InsertDiscAsync(Disc);

            if (newId <= 0)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the disc. Please try again.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
