using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class BookingDto
    {
        public long UserId { get; set; }
        public long SessionId { get; set; }
        public string SeatLabel { get; set; } = null!;
    }
}
