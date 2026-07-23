using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecordDB.DAL.Models
{
    public class Artist
    {
        public Artist() => Records = new HashSet<Record>();

        [Key]
        public int ArtistId { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(int.MaxValue), Column(TypeName = "text")]
        public string? Biography { get; set; }

        [InverseProperty(nameof(Record.Artist))]
        public virtual ICollection<Record> Records { get; set; }

        public override string ToString()
        {
            var biography = string.IsNullOrEmpty(Biography) ? "No Biography" : (Biography.Length > 30 ? Biography.Substring(0, 30) + "..." : Biography);

            return $"Artist Id: {ArtistId}, Artist: {Name}, Biography: {biography}";
        }
    }
}
