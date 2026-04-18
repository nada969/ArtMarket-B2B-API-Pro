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
                entity.HasKey(a => a.Id);       ///// define the primary key
                entity.Property(a => a.Role)
                      .HasConversion<string>();
            });
            /// ForeignKey (UserId)     Buyer(child) → User(parent) (One-to-One)
            modelBuilder.Entity<Buyer>(entity =>
            {
                entity.HasKey(a => a.BuyerId);
                entity.HasOne(a => a.User)
                      .WithOne(p => p.Buyer)
                      .HasForeignKey<Buyer>(f => f.UserId)         //// define the FK === (SQL: FOREIGN KEY (UserId) REFERENCES Users(Id))
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /// ForeignKey (UserId)       Artist(child) → User(parent) (One-to-One)
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(a => a.ArtistId);
                entity.HasOne(a => a.User)
                      .WithOne(p => p.Artist)
                      .HasForeignKey<Artist>(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /// ForeignKey (ArtistId)   Subscription(child) → Artist(parent) (One-to-One)
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(a => a.SubscriptionId);
                entity.HasOne(a => a.Artist)
                      .WithOne(p => p.Subscription)
                      .HasForeignKey<Subscription>(f => f.ArtistId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /// ForeignKey (ArtistId)       ArtWork(child) → Artist(parent) (Many-to-One)
            modelBuilder.Entity<Artwork>(entity =>
            {
                entity.HasKey(a => a.ArtId);
                entity.HasOne(a => a.Artist)
                      .WithMany(p => p.Artworks)
                      .HasForeignKey(f => f.ArtistId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /// ForeignKey (ArtId && BuyerId) 
            modelBuilder.Entity<Order>(entity => 
            {
                entity.HasKey(a => a.OrderId);

                // Order → ArtWork (Many-to-One)
                entity.HasOne(p => p.ArtWork)
                      .WithMany(f => f.Orders)
                      .HasForeignKey(f => f.ArtId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Order → Buyer (Many-to-One)
                entity.HasOne(p => p.Buyer)
                      .WithMany(f => f.Orders)
                      .HasForeignKey(f => f.BuyerId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Order → Artisr (Many-to-One)
                entity.HasOne(p => p.Artist)
                      .WithMany(f => f.Orders)
                      .HasForeignKey(f => f.ArtistId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.Status)
                      .HasConversion<string>();
            });
        }

    }
}
