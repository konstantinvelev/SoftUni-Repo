namespace SharedTrip
{
    using Microsoft.EntityFrameworkCore;
    using SharedTrip.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<UserTrip> UsersTrips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(DatabaseConfiguration.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserTrip>().HasKey(s => new { s.UserId, s.TripId });
            builder.Entity<UserTrip>(s =>
            {
                s.HasOne(a => a.Trip)
                .WithMany(e => e.UserTrips)
                .HasForeignKey(k => k.TripId)
                .OnDelete(DeleteBehavior.Restrict);

                s.HasOne(a => a.User)
               .WithMany(e => e.UserTrips)
               .HasForeignKey(k => k.UserId)
               .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
