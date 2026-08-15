using SplitBro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    public class ExpenseParticipant
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public int UserPaidId { get; set; }
        public int UserReceivedId { get; set; }
        public ExpenseSplitType SplitType { get; set; } = ExpenseSplitType.Equally;

        public Expense Expense { get; set; }
        public User UserPaid { get; set; }
        public User UserReceived { get; set; }
    }
}
