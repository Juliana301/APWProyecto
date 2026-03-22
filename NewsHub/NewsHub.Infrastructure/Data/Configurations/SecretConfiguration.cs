using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Data.Configurations
{
    public class SecretConfiguration : IEntityTypeConfiguration<SecretEnt>
    {
        public void Configure(EntityTypeBuilder<SecretEnt> builder)
        {
            builder.ToTable("Secrets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.IsEncrypted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime");

            builder.HasOne(x => x.Source)
                .WithMany(x => x.Secrets)
                .HasForeignKey(x => x.SourceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
