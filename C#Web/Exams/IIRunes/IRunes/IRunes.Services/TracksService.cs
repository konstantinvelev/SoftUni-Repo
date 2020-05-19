using IRunes.Data;
using IRunes.Models;

namespace IRunes.Services
{
    public class TracksService : ITracksService
    {
        private readonly RunesDbContext db;

        public TracksService(RunesDbContext db)
        {
            this.db = db;
        }
        public void Create(string name, string link, decimal price)
        {
            var track = new Track()
            {
                Username = name,
                Link = link,
                Price = price,
            };

            this.db.Tracks.Add(track);
            this.db.SaveChanges();
        }
    }
}
