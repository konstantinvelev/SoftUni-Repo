
using System.Collections.Generic;
using System.Linq;
using IRunes.Data;
using IRunes.Models;

namespace IRunes.Services
{
    public class AlbumsService : IAlbumsService
    {
        private readonly RunesDbContext db;

        public AlbumsService(RunesDbContext db)
        {
            this.db = db;
        }

        public void Create(string name, string cover)
        {
            var album = new Album()
            {
                Name=name,
                Cover=cover,
                Price = 0.00M,
            };
            this.db.Albums.Add(album);
            this.db.SaveChanges();
        }

        public Album GetAlbum(string id)
        {
            var album = this.db.Albums.FirstOrDefault(s => s.Id == id);

            return album;
        }

        public List<Album> GetAllAlbums()
        {
            return this.db.Albums.ToList();
        }
    }
}
