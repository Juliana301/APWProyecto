using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Data.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<LogEnt>
    {
        public void Configure(EntityTypeBuilder<LogEnt> builder)
        {
            builder.ToTable("Logs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Entity)
                .HasMaxLength(100);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
