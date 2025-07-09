using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Entities
{
    public class Recommendation
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MovieId { get; set; }
        public decimal Weight { get; set; }

        public User User { get; set; } = null!;
        public Movie Movie { get; set; } = null!;
    }
}
