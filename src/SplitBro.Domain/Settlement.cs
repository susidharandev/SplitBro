using SplitBro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Domain
{
    public class Settlement
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public int UserPaid { get; set; }
        public int UserReceived { get; set; }
        public int Amount { get; set; }
        public DateTime AddedTime { get; set; } = DateTime.UtcNow;
        public Currency currency { get; set; } = Currency.Rupee;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public User UserPay { get; set; }
        public User UserReceive { get; set; }

    }
}
