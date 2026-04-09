using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly ApplicationDbContext _context;

        public OrganizationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ClaimDomainUsersAsync(string organizationId)
        {
            var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId);
            if (org == null)
                throw new Exception("Organization not found.");

            if (!org.IsDomainVerified || string.IsNullOrEmpty(org.Domain))
                throw new Exception("Organization domain is not verified.");

            // Tìm các user có email khớp với domain và chưa được gán tổ chức nào
            var domainSuffix = "@" + org.Domain;
            var shadowUsers = await _context.Users
                .Where(u => u.Email.EndsWith(domainSuffix) && u.OrganizationId == null)
                .ToListAsync();

            foreach (var user in shadowUsers)
            {
                user.OrganizationId = org.Id;
            }

            await _context.SaveChangesAsync();
        }
    }
}
