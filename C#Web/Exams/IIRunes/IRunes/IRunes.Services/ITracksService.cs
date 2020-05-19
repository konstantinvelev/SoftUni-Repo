using System;
using System.Collections.Generic;
using System.Text;

namespace IRunes.Services
{
    public interface ITracksService
    {
        void Create(string name, string link, decimal price);
    }
}
