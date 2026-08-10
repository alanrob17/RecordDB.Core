using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordDB.Core.Pages.Discs
{
    public class IndexModel : PageModel
    {
        private readonly IDiscRepository _discRepository;

        private const int PageSize = 20;

        public IndexModel(IDiscRepository discRepository)
        {
            _discRepository = discRepository;
        }

        public IList<ArtistRecordDiscDto> Discs { get; set; } = [];

        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        /// <summary>True when the list is filtered by a search term.</summary>
        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchTerm);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            List<ArtistRecordDiscDto> all;

            if (IsSearching)
            {
                // Search routine using up_GetDiscRecordsByRecordName
                all = (await _discRepository.GetDiscRecordsByRecordNameAsync(SearchTerm!)).ToList();
            }
            else
            {
                // Full list using up_SelectAllDiscEntities
                all = (await _discRepository.SelectAllDiscEntitiesAsync()).ToList();
            }

            TotalCount  = all.Count;
            CurrentPage = Math.Max(1, pageNumber);
            TotalPages  = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            Discs = all
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
