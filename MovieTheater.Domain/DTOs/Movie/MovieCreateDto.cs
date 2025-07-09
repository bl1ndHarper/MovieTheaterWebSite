using MovieTheater.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class MovieCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public short? Duration { get; set; }
        public string? TrailerUrl { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public decimal ImdbRating { get; set; }
        public ActivityStatus ActivityStatus { get; set; }
        public long AgeRatingId { get; set; }
        public string DirectorName { get; set; } = null!;
        public string? DirectorDetailsUrl { get; set; }
    }
}
