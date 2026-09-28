using SplitBro.Application.InfraInterfaces;
using SplitBro.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SplitBro.Application.Dto.UserDto;

namespace SplitBro.Application.Service
{
    public class UserManagerService
    {
        private readonly IUserRepository _userRepository;
        public UserManagerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            // Business validation
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            // Business rule: Email must be unique
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);

            if (existingUser != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            // Ask repository to persist it
            var newUser =  await _userRepository.CreateUserAsync(user);

            if (newUser == null)
                throw new KeyNotFoundException("User not Created.");

            return ToResponse(newUser);
        }
        public async Task<UserResponse> UpdateUserAsync(int Id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(Id);

            if (user == null)
                throw new ArgumentException("User not found");

            // Business validation
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required.");

            user.Name = request.Name;
            user.Phone = request.Phone;

            await _userRepository.UpdateUserAsync(user);

            return ToResponse(user);
        }
        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return ToResponse(user);
        }
        public async Task<UserResponse> GetUserByEmailAsync(string Email)
        {
            var user = await _userRepository.GetByEmailAsync(Email);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return ToResponse(user);
        }
        private static UserResponse ToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
