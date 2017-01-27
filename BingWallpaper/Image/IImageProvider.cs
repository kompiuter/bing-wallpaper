using System.Drawing;
using System.Threading.Tasks;

namespace BingWallpaper
{
    public interface IImageProvider
    {
        Task<Image> GetImage();
    }
}
