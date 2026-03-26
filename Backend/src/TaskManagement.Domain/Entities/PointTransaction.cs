using System;

namespace TaskManagement.Domain.Entities
{
    public class PointTransaction
    {
        public Guid Id { get; set; }
        public Guid UserWalletUserId { get; set; }
        public UserWallet UserWallet { get; set; } = null!;
        public int Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
