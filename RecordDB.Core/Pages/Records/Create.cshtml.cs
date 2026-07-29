using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Records
{
    public class CreateModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        public CreateModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        [BindProperty]
        public Record Record { get; set; } = new Record { Bought = DateTime.Today, Recorded = DateTime.Today.Year };

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var newId = await _recordRepository.InsertAsync(Record);

            if (newId <= 0)
            {
                ModelState.AddModelError(string.Empty,
                    newId == 0
                        ? "This record already exists in the database."
                        : "An error occurred while saving. Please try again.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
