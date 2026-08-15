using SplitBro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ExpenseCatagory? Catagory { get; set; }
        public Currency currency { get; set; } = Currency.Rupee;
        public int Amount { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedTime { get; set; }
        public int GroupId{ get; set; }

        public Group Group { get; set; }
        public ICollection<ExpenseParticipant> ExpenseParticipants { get; set; } = new List<ExpenseParticipant>();

    }
}
