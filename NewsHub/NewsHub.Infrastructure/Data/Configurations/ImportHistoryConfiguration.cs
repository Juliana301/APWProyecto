using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Data.Configurations
{
    public class ImportHistoryConfiguration : IEntityTypeConfiguration<ImportHistory>
    {
        public void Configure(EntityTypeBuilder<ImportHistory> builder)
        {
            builder.ToTable("ImportHistory");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SourceName)
                .HasMaxLength(200);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(500);

            builder.Property(x => x.ImportedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
