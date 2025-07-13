using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class SessionCreateDto
    {
        public DateTime StartTime { get; set; }
        public int HallId { get; set; }
        public int MovieId { get; set; }
    }
}
