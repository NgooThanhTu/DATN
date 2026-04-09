using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class ProjectTemplateConfiguration : IEntityTypeConfiguration<ProjectTemplate>
    {
        public void Configure(EntityTypeBuilder<ProjectTemplate> builder)
        {
            builder.ToTable("ProjectTemplates");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
            builder.Property(t => t.TemplateCode).IsRequired().HasMaxLength(50);
            
            // Seed base templates
            builder.HasData(
                new ProjectTemplate 
                { 
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                    Name = "Basic IT service management", 
                    TemplateCode = "IT_SERVICE",
                    Description = "Dành cho Helpdesk, cung cấp sẵn các Issue Types về Service Request, Incident."
                },
                new ProjectTemplate 
                { 
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                    Name = "Software Development", 
                    TemplateCode = "SOFTWARE_DEV",
                    Description = "Dành cho Dev Team, cung cấp Scrum Board mặc định."
                }
            );
        }
    }
}
