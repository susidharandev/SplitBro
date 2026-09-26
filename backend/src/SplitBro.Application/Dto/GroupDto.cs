using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain.Enums
{
    public class GroupDto
    {
        public class CreateGroupRequest
        {
            [Required]
            public int UserId { get; set; }
            [Required]
            [StringLength(50)]
            public string Name { get; set; }

            [StringLength(500)]
            public string? Description { get; set; }
            public bool SimplifyDebt { get; set; } = false;
        }
        public class UpdateGroupRequest
        {
            [Required]
            [StringLength(50)]
            public string Name { get; set; }

            [StringLength(500)]
            public string? Description { get; set; }
            public bool SimplifyDebt { get; set; } = false;
        }
        public class CreateMemberRequest
        {
            [Required]
            public int UserId { get; set; }
            [Required]
            public int GroupId { get; set; }
        }
        public class GroupResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
        }
        public class MemberResponse
        {
            public int UserId { get; set; }
            public int GroupId { get; set; }
            public string GroupName { get; set; }
            public DateTime JoinedAt { get; set; }
        }
    }
}
