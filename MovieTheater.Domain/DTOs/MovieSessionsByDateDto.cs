using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
   public class MovieSessionsByDateDto
    {
     public DateTime Date { get; set; }
     public List<MovieMainDto> Movies { get; set; } = new();
    }
}
