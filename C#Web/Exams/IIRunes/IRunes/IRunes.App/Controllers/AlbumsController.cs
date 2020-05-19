using IRunes.App.ViewModels.Albums;
using IRunes.Models;
using IRunes.Services;
using SIS.HTTP;
using SIS.MvcFramework;
using System.Collections.Generic;
using System.Linq;

namespace IRunes.App.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly IAlbumsService albumsService;

        public AlbumsController(IAlbumsService albumsService)
        {
            this.albumsService = albumsService;
        }

        public HttpResponse All()
        {
            if (!IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            List<Album> albums = this.albumsService.GetAllAlbums();
            var albumInfo = new List<AlbumInfoViewModel>();

            foreach (var album in albums)
            {
                var currentAlbum = new AlbumInfoViewModel
                {
                    Id = album.Id,
                    Name = album.Name
                };
                albumInfo.Add(currentAlbum);
            }

            var viewModel = new AllAlbumsViewModel()
            {
                Albums = albumInfo
            };

            return this.View(viewModel, "/All");
        }
        public HttpResponse Create()
        {
            if (!IsUserLoggedIn())
            {
                this.Redirect("/Users/Login");
            }

            return this.View();

        }

        [HttpPost]
        public HttpResponse Create(CreateInputViewModel cim)
        {
            if (!IsUserLoggedIn())
            {
                this.Redirect("/Users/Login");
            }

            if (string.IsNullOrWhiteSpace(cim.Name) || string.IsNullOrWhiteSpace(cim.Cover))
            {
                this.Redirect("/Albums/Create");
            }

            this.albumsService.Create(cim.Name, cim.Cover);
            return this.Redirect("/Albums/All");
        }
        public HttpResponse Details(string id)
        {
            if (!IsUserLoggedIn())
            {
                this.Redirect("/Users/Login");
            }

            var album = this.albumsService.GetAlbum(id);

            if (album == null)
            {
                return this.Redirect("/Albums/All");
            }

            var viewModel = new DetailsAlbumViewModel
            {
                Id = album.Id,
                Cover = album.Cover,
                Name = album.Name,
                Price = (album.Tracks.Sum(s => s.Price)) * 0.87M,
                Tracks = album.Tracks.Select(e => new AlbumInfoViewModel 
                {
                    Id = e.Id,
                    Name = e.Username,
                }).ToList()
            };

            return this.View(viewModel);
        }
    }
}

