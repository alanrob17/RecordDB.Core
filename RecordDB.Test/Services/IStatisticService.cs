using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.Test.Services
{
    public interface IStatisticService
    {
        Task GetStatisticsAsync();
    }
}
