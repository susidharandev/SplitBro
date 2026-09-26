using Microsoft.EntityFrameworkCore;
using SplitBro.Application;
using SplitBro.Domain;
using SplitBro.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Infra.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly SplitAppDbContext _splitAppDbContext;
        public UserRepository(SplitAppDbContext splitAppDbContext) 
        { 
            _splitAppDbContext = splitAppDbContext;
        }
        public async Task<User> CreateUserAsync(User user)
        {
            _splitAppDbContext.Users.Add(user);
            await _splitAppDbContext.SaveChangesAsync();

            return user;
        }
        public Task UpdateUserAsync(User user)
        {
            _splitAppDbContext.Update(user);
            return _splitAppDbContext.SaveChangesAsync();
        }
        public Task<User?> GetByEmailAsync(string email)
        {
            return _splitAppDbContext.Users.FirstOrDefaultAsync(x=> x.Email == email);
        }
        public Task<User?> GetByIdAsync(int id)
        {
            return _splitAppDbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

    }
}
