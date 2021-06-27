using Serilog;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace StealthScreenCapture
{
    static class ScreenCaptureUtils
    {
        public static Bitmap CaptureScreen()
        {
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;
            Bitmap memoryImage = new Bitmap(width, height);
            Size s = new Size(width, height);
            using (Graphics graphics = Graphics.FromImage(memoryImage))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, s, CopyPixelOperation.SourceCopy);
            }
            return memoryImage;
        }

        public static void SaveImageWithDatetime(Bitmap image, ImageCodecInfo imageCodecInfo, EncoderParameters encoderParameters)
        {
            string path = Path.Join(Directory.GetCurrentDirectory(), "screenshot", DateTime.Now.ToString("yyyy-MM-dd"));
            string filename = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
            try
            {
                if (!Directory.Exists(path))
                {
                    Log.Information($"Directory {path} does not exists. Attempting to create one...");
                    Directory.CreateDirectory(path);
                }
                image.Save(Path.Join(path, filename), imageCodecInfo, encoderParameters);
                Log.Debug($"Image {filename} is created successfully");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
