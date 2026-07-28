using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public interface IStatisticRepository
    {
        Task<Statistic> GetStatisticsAsync();
    }
}
