using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.DTOs
{
    public class ArtistRecordDiscDto
    {
        public int RecordId { get; set; }

        public int DiscId { get; set; }
        
        public string? ArtistName { get; set; }

        public string? Name { get; set; }

        public int DiscNo { get; set; }

        public int? FreeDbDiscId { get; set; }

        public string? FreeDbId { get; set; }

        public int? Length { get; set; }
    }
}
