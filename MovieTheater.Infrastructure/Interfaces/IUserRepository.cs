using MovieTheater.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(long id);
    
    }

}
