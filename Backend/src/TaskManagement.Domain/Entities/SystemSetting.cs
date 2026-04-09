using System;

namespace TaskManagement.Domain.Entities
{
    public class SystemSetting
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string SettingGroup { get; set; } = "System"; // e.g. "System", "Security"
        public string? Description { get; set; }
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
