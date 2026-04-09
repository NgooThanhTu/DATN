using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class ProjectDepartmentRoleConfiguration : IEntityTypeConfiguration<ProjectDepartmentRole>
    {
        public void Configure(EntityTypeBuilder<ProjectDepartmentRole> builder)
        {
            builder.ToTable("ProjectDepartmentRoles");
            
            // Composite Key
            builder.HasKey(pdr => new { pdr.ProjectId, pdr.DepartmentId, pdr.RoleName });

            builder.Property(pdr => pdr.RoleName).IsRequired().HasMaxLength(100);

            builder.HasOne(pdr => pdr.Project)
                .WithMany(p => p.ProjectDepartmentRoles)
                .HasForeignKey(pdr => pdr.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pdr => pdr.Department)
                .WithMany(d => d.ProjectDepartmentRoles)
                .HasForeignKey(pdr => pdr.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
