using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RecordDB.DAL.Models
{
    public class Disc
    {
        public Disc()
        {
            Tracks = new HashSet<Track>();
        }

        public int DiscId { get; set; }
        public int RecordId { get; set; }
        public int DiscNo { get; set; }
        public int? FreeDbDiscId { get; set; }
        public string? FreeDbId { get; set; }
        public int? Length { get; set; }

        public virtual Record Record { get; set; } = null!;
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
