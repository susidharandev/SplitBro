using SplitBro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    internal class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Enum? Catagory { get; set; }
        public Enum currency { get; set; } = Currency.Rupee;
        public int Amount { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedTime { get; set; }
        public int GroupId{ get; set; }
    }
}
