using Microsoft.EntityFrameworkCore;
using ShortUrl.Entities;

namespace ShortUrl.DbPersistence
{
    public class ShortUrlContext : DbContext
    {
        public ShortUrlContext(DbContextOptions<ShortUrlContext> options) : base(options)
        {
        }

        public DbSet<ApiKeyEntity> ApiKeys => Set<ApiKeyEntity>();
        public DbSet<ShortUrlEntity> ShortUrls => Set<ShortUrlEntity>();
        public DbSet<ClickEventEntity> ClickEvents => Set<ClickEventEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApiKeyEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();

                entity.Property(e => e.ApiKey).IsRequired();
                entity.HasIndex(e => e.ApiKey).IsUnique()
                    .HasDatabaseName("IX_ApiKey_Unique");

                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CanSetCustomShortCodes).IsRequired()
                    .HasDefaultValue(false);

                // Navigation property
                entity.HasMany(e => e.ShortUrls)
                      .WithOne(s => s.Owner)
                      .HasForeignKey(s => s.OwnerId);
            });

            modelBuilder.Entity<ShortUrlEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();

                entity.Property(e => e.ShortCode).IsRequired();
                entity.HasIndex(e => e.ShortCode).IsUnique()
                    .HasDatabaseName("IX_ShortUrl_ShortCode_Unique");

                entity.Property(e => e.LongUrl).IsRequired();
                entity.Property(e => e.OwnerId).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();

                // Navigation property
                entity.HasMany(e => e.ClickEvents)
                      .WithOne(c => c.ShortUrl)
                      .HasForeignKey(c => c.ShortUrlId);
            });

            modelBuilder.Entity<ClickEventEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();

                entity.Property(e => e.ShortUrlId).IsRequired();
            });
        }
    }
}
