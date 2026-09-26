using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Application.Dto
{
    public class Dto
    {
        public class CreateUserRequest
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Email { get; set; } 
            public string? Phone { get; set; }
        }
        public class UpdateUserRequest
        {
            [Required]
            public string Name { get; set; } 
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
