namespace Cinema.Data
{
    using Cinema.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class CinemaContext : DbContext
    {
        public CinemaContext() { }

        public CinemaContext(DbContextOptions options)
            : base(options) { }


        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Projection>(entity =>
            {
                entity.HasOne(s => s.Movie)
                .WithMany(a => a.Projections)
                .HasForeignKey(k => k.MovieId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(s => s.Hall)
                .WithMany(a => a.Projections)
                .HasForeignKey(k => k.HallId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Seat>(seat =>
            {
                seat.HasOne(p => p.Hall)
                .WithMany(s => s.Seats)
                .HasForeignKey(w => w.HallId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Ticket>(ticket =>
            {
                ticket.HasOne(a => a.Customer)
                .WithMany(s => s.Tickets)
                .HasForeignKey(k => k.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);


                ticket.HasOne(a => a.Projection)
                .WithMany(s => s.Tickets)
                .HasForeignKey(k => k.ProjectionId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}