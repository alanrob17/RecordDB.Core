using RecordDB.DAL.Data;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public class TotalRepository : ITotalRepository
    {
        private readonly IDataAccess _db;

        public TotalRepository(IDataAccess db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IEnumerable<Total>> GetTotalCosts()
        {
            var sproc = "sp_getTotalsForEachArtist";

            var totals = await _db.GetData<Total, dynamic>(sproc, new { });

            return totals.ToList();
        }
    }
}
