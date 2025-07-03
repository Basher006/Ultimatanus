using BotFW_CvSharp_01;
using OpenCvSharp;

namespace Unltimanus.App
{
    public class ImageFindResult
    {
        public string Name;
        public double Match;
    }

    public static class ImageFind
    {
        public static ImageFindResult FindImage(Mat screen, Mat template)
        {
            // chek screen and template
            if (screen == null || screen.IsDisposed)
                throw new Exception("Invalid screen!");
            if (screen.Channels() != template.Channels())
                throw new Exception("Screen number of chanels != template number of chanels!");
            if (template == null || template.IsDisposed)
                throw new Exception("Invalid template!");
            if (screen.Width <= 0 || screen.Height <= 0)
                throw new Exception("Screen invalid dims!");
            if (template.Width < screen.Width || template.Height <= screen.Height)
                throw new Exception("Template invalid dims!");

            var res = BotFW.MatchTemplate(screen, template);

            return default;
        }

        public static ImageFindResult FindImage(Mat screen, Mat template, float tr)
        {
            // chek screen and template
            if (screen == null || screen.IsDisposed)
                throw new Exception("Invalid screen!");
            if (screen.Channels() != template.Channels())
                throw new Exception("Screen number of chanels != template number of chanels!");
            if (template == null || template.IsDisposed)
                throw new Exception("Invalid template!");
            if (screen.Width <= 0 || screen.Height <= 0)
                throw new Exception("Screen invalid dims!");
            if (template.Width < screen.Width || template.Height <= screen.Height)
                throw new Exception("Template invalid dims!");

            var res = BotFW.MatchTemplate(screen, template, tr);

            return default;
        }
    }
}
