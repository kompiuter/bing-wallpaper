namespace BingWallpaper
{
    /// <summary>
    /// Wallpaper控制接口
    /// </summary>
    public interface IWallpaperControl
    {
        HistoryImage CurrentWallpaper { get; set; }

        event MainForm.WallpaperChangeHandler OnWallpaperChange;

        void NextWallpaper();
        void PreWallpaper();
        void RandomWallpaper();
    }
}