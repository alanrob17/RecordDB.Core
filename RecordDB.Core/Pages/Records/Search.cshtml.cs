using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RecordDB.Core.Pages.Records
{
    public class SearchModel : PageModel
    {
        private readonly IRecordRepository _recordRepository;

        public SearchModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Record Name")]
        public string? FilterName { get; set; }

        [BindProperty]
        [Display(Name = "Select Record")]
        [Required(ErrorMessage = "Please select a record from the dropdown list.")]
        public int SelectedRecordId { get; set; }

        public List<ArtistRecordDto> AllRecords { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            var records = await _recordRepository.SelectAsync();
            AllRecords = records.OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedRecordId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a record from the dropdown list.");
                var records = await _recordRepository.SelectAsync();
                AllRecords = records.OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase).ToList();
                return Page();
            }

            // Retrieve record details using up_RecordSelectByIdCore via RecordRepository
            ArtistRecordDto? record = await _recordRepository.SelectAsync(SelectedRecordId);

            if (record == null)
            {
                ModelState.AddModelError(string.Empty, "The selected record could not be found.");
                var records = await _recordRepository.SelectAsync();
                AllRecords = records.OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase).ToList();
                return Page();
            }

            return RedirectToPage("/Records/RecordView", new { id = record.RecordId });
        }
    }
}
