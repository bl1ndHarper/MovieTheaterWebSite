using MovieTheater.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class BookingDetailsDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long SessionId { get; set; }
        public long SessionSeatId { get; set; }
        public BookingStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
