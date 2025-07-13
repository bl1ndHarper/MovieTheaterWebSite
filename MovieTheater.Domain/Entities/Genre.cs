using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Entities
{
    public class Genre
    {
        
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<MovieGenre> Movies { get; set; } = new List<MovieGenre>();
    }
}
