using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool SimplifyDebt { get; set; } = false;
        public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt{ get; set;} = DateTime.UtcNow;

        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
