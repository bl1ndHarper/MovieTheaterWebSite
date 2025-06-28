using MovieTheater.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Entities
{
    public class SessionSeat
    {
        public long Id { get; set; }

        public long SessionId { get; set; }
        public Session Session { get; set; } = null!;

        public long HallSeatId { get; set; }
        public HallSeat HallSeat { get; set; } = null!;

        public HallSeatStatus Status { get; set; } = HallSeatStatus.Free;
    }

}
