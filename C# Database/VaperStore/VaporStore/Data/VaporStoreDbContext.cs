namespace VaporStore.Data
{
	using Microsoft.EntityFrameworkCore;
    using VaporStore.Data.Models;

    public class VaporStoreDbContext : DbContext
	{
		public VaporStoreDbContext()
		{
		}

		public VaporStoreDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Card> Cards{ get; set; }
        public DbSet<Developer> Developers{ get; set; }
        public DbSet<Game> Games{ get; set; }
        public DbSet<GameTag> GameTags{ get; set; }
        public DbSet<Genre> Genres{ get; set; }
        public DbSet<Purchase> Purchases{ get; set; }
        public DbSet<Tag> Tags{ get; set; }
        public DbSet<User> Users{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			if (!options.IsConfigured)
			{
				options
					.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder model)
		{
            model.Entity<Card>(card =>
            {
                card.HasOne(u => u.User)
                .WithMany(c => c.Cards)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            model.Entity<Game>(game =>
            {
                game.HasOne(d => d.Developer)
                .WithMany(g => g.Games)
                .HasForeignKey(k => k.DeveloperId)
                .OnDelete(DeleteBehavior.Restrict);

                game.HasOne(d => d.Genre)
                .WithMany(g => g.Games)
                .HasForeignKey(k => k.GenreId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            model.Entity<GameTag>(gt =>
            {
                gt.HasKey(k => new { k.GameId, k.TagId });

                gt.HasOne(p => p.Tag)
                .WithMany(s => s.GameTags)
                .HasForeignKey(k => k.TagId)
                .OnDelete(DeleteBehavior.Restrict);

                gt.HasOne(d => d.Game)
                .WithMany(g => g.GameTags)
                .HasForeignKey(k => k.GameId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            model.Entity<Purchase>(entity =>
            {
                entity.HasOne(s => s.Game)
                .WithMany(s => s.Purchases)
                .HasForeignKey(k => k.GameId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Card)
                .WithMany(g => g.Purchases)
                .HasForeignKey(k => k.CardId)
                .OnDelete(DeleteBehavior.Restrict);
            });
		}
	}
}