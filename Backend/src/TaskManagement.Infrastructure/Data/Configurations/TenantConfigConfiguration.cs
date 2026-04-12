using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class TenantConfigConfiguration : IEntityTypeConfiguration<TenantConfig>
    {
        public void Configure(EntityTypeBuilder<TenantConfig> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.OrganizationName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Domain).HasMaxLength(255);
            builder.Property(t => t.LogoUrl).HasMaxLength(1000);
            
            // Seed default organization record
            builder.HasData(new TenantConfig 
            { 
                Id = System.Guid.Parse("10000000-0000-0000-0000-000000000001"), 
                OrganizationName = "Global Organization",
                Require2FA = false,
                AllowContact = true
            });
        }
    }
}
