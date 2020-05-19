using System.Collections;
using System.Collections.Generic;

namespace IRunes.App.ViewModels.Albums
{
    public class DetailsAlbumViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Cover { get; set; }

        public decimal Price { get; set; }

        public List<AlbumInfoViewModel> Tracks { get; set; }
    }
}
