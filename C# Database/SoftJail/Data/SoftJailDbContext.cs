namespace SoftJail.Data
{
    using Microsoft.EntityFrameworkCore;
    using SoftJail.Data.Models;

    public class SoftJailDbContext : DbContext
    {
        public SoftJailDbContext()
        {
        }

        public SoftJailDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Cell> Cells { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Mail> Mails { get; set; }

        public DbSet<Officer> Officers { get; set; }

        public DbSet<OfficerPrisoner> OfficersPrisoners { get; set; }

        public DbSet<Prisoner> Prisoners { get; set; }


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
            builder.Entity<Cell>(cell =>
            {
                cell.HasOne(p => p.Department)
                .WithMany(c => c.Cells)
                .HasForeignKey(k => k.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Mail>(entity =>
            {
                entity.HasOne(p => p.Prisoner)
                .WithMany(s => s.Mails)
                .HasForeignKey(k => k.PrisonerId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Officer>(off =>
            {
                off.HasOne(o => o.Department);
            });

            builder.Entity<OfficerPrisoner>(op =>
            {
                op.HasKey(k => new { k.PrisonerId, k.OfficerId });

                op.HasOne(p => p.Officer)
                .WithMany(a => a.OfficerPrisoners)
                .HasForeignKey(k => k.OfficerId)
                .OnDelete(DeleteBehavior.Restrict);

                op.HasOne(p => p.Prisoner)
                .WithMany(a => a.PrisonerOfficers)
                .HasForeignKey(k => k.PrisonerId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Prisoner>(prisoner =>
            {
                prisoner.HasOne(x => x.Cell)
                .WithMany(d => d.Prisoners)
                .HasForeignKey(k => k.CellId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}