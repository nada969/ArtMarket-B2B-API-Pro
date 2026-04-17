using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Data
{
    public class AppDb:DbContext
    {
        public AppDb(DbContextOptions<AppDb> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Order> Orders { get; set; }

        /// Add Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /// PrimaryKey (UserId) ---> must show (Role)
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Role)
                      .HasConversion<string>();
            });
            /// ForeignKey (UserId)
            modelBuilder.Entity<Buyer>(entity =>
            {
                entity.HasKey(a => a.UserId);

                entity.HasOne(a => a.User)
                      .WithOne(p => p.Buyer)
                      .HasForeignKey<Buyer>(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            /// ForeignKey (UserId)
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(a => a.UserId);
                entity.HasOne(a => a.User)
                      .WithOne(p => p.Artist)
                      .HasForeignKey<Artist>(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            /// ForeignKey (ArtistId)
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(a => a.ArtistId);
                entity.HasOne(a => a.Artist)
                      .WithOne(p => p.Subscription)
                      .HasForeignKey<Subscription>(f => f.ArtistId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}
