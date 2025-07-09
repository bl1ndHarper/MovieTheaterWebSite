using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class SessionDailyCreateDto
    {
        public long MovieId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public Hall Hall { get; set; } = null!;
        public short SeatsTotal { get; set; }
    }
}
