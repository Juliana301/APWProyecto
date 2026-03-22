using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEnt>
    {
        public void Configure(EntityTypeBuilder<UserEnt> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.UserName)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}