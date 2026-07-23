using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.Models
{
    public class Total
    {
        #region " Properties "

        public int ArtistId { get; set; }

        public string? Name { get; set; }

        public int TotalDiscs { get; set; }

        public decimal TotalCost { get; set; }

        # endregion
    }
}
