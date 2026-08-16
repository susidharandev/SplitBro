using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain.Enums
{
    public class GroupDto
    {
        public class CreateGroupRequest
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public bool SimplifyDebt { get; set; } = false;
        }
        public class UpdateGroupRequest
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public bool SimplifyDebt { get; set; } = false;
        }
        public class GroupResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
        }
        public class CreateMemberRequest
        {
            public int UserId { get; set; }
            public int GroupId { get; set; }
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
