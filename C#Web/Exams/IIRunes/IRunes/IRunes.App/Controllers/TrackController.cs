using IRunes.App.ViewModels.Tracks;
using IRunes.Services;
using SIS.HTTP;
using SIS.MvcFramework;

namespace IRunes.App.Controllers
{
    public class TrackController : Controller
    {
        private readonly ITracksService tracksService;

        public TrackController(ITracksService tracksService)
        {
            this.tracksService = tracksService;
        }

        [HttpGet(url: "/Tracks/Create")]
        public HttpResponse Create(string albumId)
        {
            if (!IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }
            

            return this.View(new AbumIdInputModel { AlbumId = albumId });
        }

        [HttpPost]
        public HttpResponse Create(CreateTrackInputModel input)
        {
            if (!IsUserLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            if (input.Name.Length <= 4 || input.Name.Length > 10)
            {
                return this.Redirect("/Tracks/Create");
            }

            if (string.IsNullOrWhiteSpace(input.Link))
            {
                return this.Redirect("/Tracks/Create");
            }

            if (input.Price < 0.00M)
            {
                return this.Redirect("/Tracks/Create");
            }

            this.tracksService.Create(input.Name, input.Link, input.Price);

            return this.View();
        }
    }
}
