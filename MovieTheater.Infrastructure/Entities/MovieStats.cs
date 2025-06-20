using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Entities
{
    public class MovieStats
    {
        public long Id { get; set; }
        public long MovieId { get; set; }
        public DateTime Date { get; set; }
        public int TicketsSold { get; set; }
        public decimal Revenue { get; set; }
        public int TotalViewersCount { get; set; }

        public Movie Movie { get; set; } = null!;
    }
}
