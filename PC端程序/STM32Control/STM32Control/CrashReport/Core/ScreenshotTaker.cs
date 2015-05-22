using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace STM32Control.CrashReport.Core
{
    public static class ScreenshotTaker
    {
        public const string ScreenshotMimeType = "image/jpeg";

        public static Bitmap TakeScreenShot()
        {
            var rectangle = Rectangle.Empty;

            foreach (var screen in Screen.AllScreens)
            {
                rectangle = Rectangle.Union(rectangle, screen.Bounds);
            }

            var bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, rectangle.Size, CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        public static string GetImageAsFile(Bitmap bitmap)
        {
            ScreenshotFileName name = new ScreenshotFileName();
            var tempFileName = System.IO.Path.GetTempPath() + name.FileName(); //ScreenshotFileName;
            bitmap.Save(tempFileName, ImageFormat.Jpeg);
            return tempFileName;
        }
    }

    public class ScreenshotFileName
    {
        public string FileName()
        {
            string ScreenshotFileName = "ExceptionReport_Screenshot" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
            return ScreenshotFileName;
        }
    }
}
