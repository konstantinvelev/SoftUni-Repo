
namespace Panda.Data
{
    using Microsoft.EntityFrameworkCore;
    using Panda.Data.Models;
    using Panda.Models;
    using System;
    public class PandaDbContex : DbContext
    {
        public PandaDbContex()
        {}

        public DbSet<User> Users { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(DbSettings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasOne(p => p.Recipient)
                .WithMany(r => r.Packages)
                .HasForeignKey(p => p.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.HasOne(r => r.Recipient)
                .WithMany(p => p.Receipts)
                .HasForeignKey(r => r.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
