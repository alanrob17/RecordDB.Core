using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RecordDB.Core.Pages.Records
{
    public class RecordedModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Year is required.")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Year must be a 4-digit number.")]
        public int? Year { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            return RedirectToPage("/Records/GetRecords", new { show = $"r{Year}" });
        }
    }
}
