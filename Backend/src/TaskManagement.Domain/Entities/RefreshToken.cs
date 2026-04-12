using System;

namespace TaskManagement.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Token { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}
