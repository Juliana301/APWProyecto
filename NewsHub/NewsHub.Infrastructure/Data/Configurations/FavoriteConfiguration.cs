using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Data.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<FavoriteEnt>
    {
        public void Configure(EntityTypeBuilder<FavoriteEnt> builder)
        {
            builder.ToTable("Favorites");

            builder.HasKey(x => new { x.UserId, x.SourceItemId });

            builder.HasOne(x => x.User)
                .WithMany(x => x.Favorites)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.SourceItem)
                .WithMany(x => x.Favorites)
                .HasForeignKey(x => x.SourceItemId);

            builder.Property(x => x.AddedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}