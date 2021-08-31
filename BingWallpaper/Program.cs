using System;
using System.Windows.Forms;
using System.Threading;

namespace BingWallpaper
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(false, "b1c063de-2104-468e-ab02-4ca06b0c213e");
            // Check if application is already running
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        SetProcessDPIAware();
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    // Run the application
                    Application.Run(new MainForm(new BingImageProvider(), new Settings()));
                }
                else
                {
                    MessageBox.Show("An instance of the application is already running", "Bing Wallpaper");
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
