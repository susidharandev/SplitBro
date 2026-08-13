using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    internal class GroupMemeber
    {
        public int UserId { get; set; }
        public int groupId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
