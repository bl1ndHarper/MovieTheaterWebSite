using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }
}
