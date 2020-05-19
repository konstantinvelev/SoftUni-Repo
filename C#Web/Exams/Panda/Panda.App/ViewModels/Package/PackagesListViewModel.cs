using System;
using System.Collections.Generic;
using System.Text;

namespace Panda.App.ViewModels.Package
{
  public  class PackagesListViewModel
    {
        public IEnumerable<PackageViewModel> Packages { get; set; }
    }
}
