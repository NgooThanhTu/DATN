using System;
using System.Collections.Generic;

namespace TaskManagement.Application.DTOs.Admin
{
    public class UserAdminDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserAdminDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
    }
}
