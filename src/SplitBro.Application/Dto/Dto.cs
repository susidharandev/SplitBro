using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Application.Dto
{
    public class Dto
    {
        public class CreateUserRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
        }
        public class UpdateUserRequest
        {
            public string Name { get; set; } = string.Empty;
            public string? Phone { get; set; }
        }
        public class UserResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTime CreatedAt { get; set; } 
        }
    }
}
