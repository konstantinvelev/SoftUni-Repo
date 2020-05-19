using IRunes.Models;
using System.Collections.Generic;

namespace IRunes.Services
{
    public interface IAlbumsService
    {
        List<Album> GetAllAlbums();

        void Create(string name,string cover);

        Album GetAlbum(string id);
    }
}
