using BotFW_CvSharp_01;
using BotFW_CvSharp_01.CvThings;
using OpenCvSharp;

namespace Unltimanus.App
{
    public class ModFinderResult
    {
        public string Name;
        public double Match;

        public override string ToString()
        {
            return $"{Name}({Match})";
        }
    }

    public static class ModFinder
    {
        public static ModFinderResult FindMostMathMod(Mat screen, Dictionary<string, Mat> templates)
        {
            var results = FindAllTemplates(screen, templates);
            var mostMatchResult = FindMaxRes(results);
            return mostMatchResult;
        }

        private static Dictionary<string, ResLoc> FindAllTemplates(Mat screen, Dictionary<string, Mat> templates)
        {
            var results = new Dictionary<string, ResLoc>();
            foreach (var t in templates)
            {
                var res_first = FindImage(screen, t.Value);
                results.Add(t.Key, res_first.MaxResLoc);
            }

            return results;
        }

        public static MT_result FindImage(Mat screen, Mat template)
        {
            // chek screen and template
            if (screen == null || screen.IsDisposed)
                throw new Exception("Invalid screen!");
            if (screen.Channels() != template.Channels())
                throw new Exception("Screen number of chanels not equal template number of chanels!");
            if (template == null || template.IsDisposed)
                throw new Exception("Invalid template!");
            if (screen.Width <= 0 || screen.Height <= 0)
                throw new Exception("Screen invalid size!");
            if (template.Width > screen.Width || template.Height > screen.Height)
                throw new Exception("Template invalid size!");

            return BotFW.MatchTemplate(screen, template);
        }

        private static ModFinderResult FindMaxRes(Dictionary<string, ResLoc> res)
        {
            string maxKey = "null";
            double maxVal = 0.0;
            foreach (var r in res)
            {
                if (r.Value.Value > maxVal)
                {
                    maxKey = r.Key;
                    maxVal = r.Value.Value;
                }
            }

            return new ModFinderResult()
            {
                Name = maxKey,
                Match = Math.Round(maxVal, 2)
            };
        }
    }
}
