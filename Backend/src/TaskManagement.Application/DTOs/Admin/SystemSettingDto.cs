using System;

namespace TaskManagement.Application.DTOs.Admin
{
    public class SystemSettingDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string SettingGroup { get; set; } = string.Empty;
    }
}
