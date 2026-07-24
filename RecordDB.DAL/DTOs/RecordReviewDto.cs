using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.DAL.DTOs
{
    public class RecordReviewDto
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string Review { get; set; } = string.Empty;
    }
}
