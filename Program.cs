using System.IO;
using Serilog;
using System.Threading;
using System.Drawing.Imaging;
using Microsoft.Extensions.Configuration;
using System;

namespace StealthScreenCapture
{
    class Program
    {
        static int CaptureInterval;
        static ImageCodecInfo Codec;
        static EncoderParameters ImageEncoderParameters;
        static void loadLogger()
        {
            string logFile = Path.Join(Directory.GetCurrentDirectory(), "logs.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(logFile)
                .CreateLogger();
            Log.Debug("Logger is now configured");
        }

        static void loadConfig()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            CaptureInterval = config.GetValue<int>("SSC:CaptureInterval");
            long imageQuality = config.GetValue<long>("SSC:ImageQuality");
            Log.Information($"Capture Interval: {CaptureInterval}s, Image Quality: {imageQuality}");
            ImageFormat format = ImageFormat.Jpeg;
            Codec = Array.Find(ImageCodecInfo.GetImageEncoders(), encoder => encoder.FormatID == format.Guid);
            ImageEncoderParameters = new EncoderParameters(1);
            ImageEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, imageQuality);
        }

        static bool stopCondition()
        {
            string stopFile = Path.Join(Directory.GetCurrentDirectory(), "stop.txt");
            if (File.Exists(stopFile))
            {
                Log.Information($"Stop condition is triggered");
                File.Delete(stopFile);
                return true;
            }
            return false;
        }

        static void Main(string[] args)
        {
            loadLogger();
            loadConfig();
            while (!stopCondition())
            {
                using (var bitmap = ScreenCaptureUtils.CaptureScreen())
                {
                    ScreenCaptureUtils.SaveImageWithDatetime(bitmap, Codec, ImageEncoderParameters);
                }
                Thread.Sleep(CaptureInterval * 1000);
            }
            
        }
    }
}
