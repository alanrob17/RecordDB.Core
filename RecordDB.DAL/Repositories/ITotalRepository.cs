using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public interface ITotalRepository
    {
        Task<IEnumerable<Total>> GetTotalCosts();
    }
}
