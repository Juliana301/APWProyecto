using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Data.Configurations
{
    public class SourceItemConfiguration : IEntityTypeConfiguration<SourceItemEnt>
    {
        public void Configure(EntityTypeBuilder<SourceItemEnt> builder)
        {
            builder.ToTable("SourceItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Json)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime");

            builder.HasOne(x => x.Source)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.SourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
