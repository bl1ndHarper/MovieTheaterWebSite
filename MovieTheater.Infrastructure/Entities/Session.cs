using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Entities
{
    public class Session
    {
        public long Id { get; set; }
        public long MovieId { get; set; }
        public long HallId { get; set; }
        public DateTime StartTime { get; set; }
        public short SeatsTotal { get; set; }

        public Movie Movie { get; set; } = null!;
        public Hall Hall { get; set; } = null!;
    }
}
