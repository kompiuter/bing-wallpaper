using System;
using System.IO;

namespace BingWallpaper
{
    public static class Log
    {
        public static void Print(string text)
        {
            using (var sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
            {
                sw.WriteLine($"[{DateTime.UtcNow.ToString("s")}] {text}");
                sw.Flush();
            }
        }

        public static void Print(Exception e)
        {
            using (var sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
            {
                sw.WriteLine($"[{DateTime.UtcNow.ToString("s")}] {e.Source.ToString()}: {e.Message.ToString()}");
                sw.Flush();
            }
        }
    }
}
