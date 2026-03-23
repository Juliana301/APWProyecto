using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Data.Configurations
{
    public class SourceConfiguration : IEntityTypeConfiguration<SourceEnt>
    {
        public void Configure(EntityTypeBuilder<SourceEnt> builder)
        {
            builder.ToTable("Source");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.ComponentType)
                .HasConversion<string>();

            builder.HasIndex(x => x.ComponentType);

            builder.Property(x => x.RequiresSecret)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.ApiConfigJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);
        }
    }
}
