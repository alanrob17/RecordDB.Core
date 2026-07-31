using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;

namespace RecordDB.Core.Pages.Statistics
{
    public class IndexModel : PageModel
    {
        private readonly IStatisticRepository _statisticRepository;

        public IndexModel(IStatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }

        public Statistic Statistics { get; set; }

        public async Task OnGetAsync()
        {
            Statistics = await _statisticRepository.GetStatisticsAsync();
        }
    }
}
