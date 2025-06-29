using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class MovieMainDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public string? Genre { get; set; }
        public List<string> Genres { get; set; } = new();
        public string? AgeRating { get; set; }
        public short? Duration { get; set; }
        public decimal ImdbRating { get; set; }
        public List<string> Sessions { get; set; } = new();
        
    }
}
