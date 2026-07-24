using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecordDB.DAL.DTOs
{
    public class ArtistRecordDto
    {
        public int RecordId { get; set; }

        public int ArtistId { get; set; }

        public string? ArtistName { get; set; }

        public string? Name { get; set; }

        public string? Field { get; set; }

        public int Recorded { get; set; }

        public string? Label { get; set; }

        public string? Pressing { get; set; }

        public string? Rating { get; set; }

        public int Discs { get; set; }

        public string? Media { get; set; }

        public string? Bought { get; set; }

        public decimal? Cost { get; set; }

        public string? Review { get; set; }
    }
}
