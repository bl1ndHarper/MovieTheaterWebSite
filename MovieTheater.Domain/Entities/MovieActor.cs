using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Entities
{
    public class MovieActor
    {
        public long MovieId { get; set; }
        public long ActorId { get; set; }

        public Movie Movie { get; set; } = null!;
        public Actor Actor { get; set; } = null!;
    }
}
