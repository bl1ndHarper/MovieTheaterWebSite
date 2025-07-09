using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Entities
{
    public class Actor
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? DetailsUrl { get; set; }

        public ICollection<MovieActor> Movies { get; set; } = new List<MovieActor>();
    }
}
