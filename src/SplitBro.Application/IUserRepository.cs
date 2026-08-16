using SplitBro.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Application
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        
        // deletUser
    }
}
