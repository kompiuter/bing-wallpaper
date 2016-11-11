using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper
{
    interface IImageProvider
    {
        Task<Uri> Uri();
    }
}
