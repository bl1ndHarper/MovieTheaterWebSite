using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class MovieSessionDto
    {
        public long Id { get; set; }
        public long MovieId { get; set; }
        public Hall Hall { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public short SeatsTotal { get; set; }
    }
}
