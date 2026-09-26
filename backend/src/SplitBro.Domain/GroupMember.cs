using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    public class GroupMember
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Group Group { get; set; }
    }
}
