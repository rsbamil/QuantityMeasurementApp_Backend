using Microsoft.EntityFrameworkCore;
using QuantityMeasurementAppModelLayer.Models;

namespace QuantityMeasurementAppRepositoryLayer.Data
{
    public class QuantityMeasurementDbContext : DbContext
    {
        public QuantityMeasurementDbContext(DbContextOptions<QuantityMeasurementDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<QuantityMeasurementEntity> QuantityMeasurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users table config
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // QuantityMeasurements table config
            modelBuilder.Entity<QuantityMeasurementEntity>(entity =>
            {
                entity.Property(q => q.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Relationship: a measurement optionally belongs to a user
                entity.HasOne(q => q.User)
                      .WithMany(u => u.Measurements)
                      .HasForeignKey(q => q.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}