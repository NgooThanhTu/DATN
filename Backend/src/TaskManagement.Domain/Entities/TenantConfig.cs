using System;

namespace TaskManagement.Domain.Entities
{
    public class TenantConfig
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public string? LogoUrl { get; set; }
        public bool Require2FA { get; set; } = false;
        public bool AllowContact { get; set; } = true;
        public bool PublicProfile { get; set; } = false;
        public string? AllowedContactTopics { get; set; } // JSON list
        public string? IpWhitelist { get; set; } // JSON string array
    }
}
