using SplitBro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    internal class ExpenseSplit
    {
        public int Id { get; set; }
        public string ExpenseId { get; set; }
        public int UserPaid { get; set; }
        public int UserReceived { get; set; }
        public Enum SplitType { get; set; } = ExpenseSplitType.Equally;
    }
}
