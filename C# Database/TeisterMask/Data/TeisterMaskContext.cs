namespace TeisterMask.Data
{
    using Microsoft.EntityFrameworkCore;
    using TeisterMask.Data.Models;

    public class TeisterMaskContext : DbContext
    {
        public TeisterMaskContext() { }

        public TeisterMaskContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeTask> EmployeesTasks { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Task> Tasks { get; set; }

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
            builder.Entity<EmployeeTask>(et =>
            {
                et.HasKey(k => new { k.EmployeeId, k.TaskId });

                et.HasOne(e => e.Employee)
                .WithMany(t => t.EmployeesTasks)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

                et.HasOne(e => e.Task)
                .WithMany(t => t.EmployeesTasks)
                .HasForeignKey(p => p.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Task>(t => 
            {
                t.HasOne(p => p.Project)
                .WithMany(s => s.Tasks)
                .HasForeignKey(k => k.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}