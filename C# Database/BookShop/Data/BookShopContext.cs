namespace BookShop.Data
{
    using BookShop.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BookShopContext : DbContext
    {
        public BookShopContext() { }

        public BookShopContext(DbContextOptions options)
            : base(options) { }


        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<AuthorBook> AuthorsBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorBook>(ab =>
            {
                ab.HasKey(k => new { k.AuthorId, k.BookId });

                ab.HasOne(p => p.Book)
                .WithMany(s => s.AuthorsBooks)
                .HasForeignKey(a => a.BookId)
                .OnDelete(DeleteBehavior.Restrict);


                ab.HasOne(p => p.Author)
                .WithMany(s => s.AuthorsBooks)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}