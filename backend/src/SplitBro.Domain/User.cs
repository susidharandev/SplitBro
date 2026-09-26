using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone{ get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<ExpenseParticipant> ExpenseParticipantsPaid { get; set; } = new List<ExpenseParticipant>();
        public ICollection<ExpenseParticipant> ExpenseParticipantsReceived { get; set; } = new List<ExpenseParticipant>();
        public ICollection<Settlement> SettlementsPaid { get; set; } = new List<Settlement>();
        public ICollection<Settlement> SettlementsReceived { get; set; } = new List<Settlement>();
    }
}
