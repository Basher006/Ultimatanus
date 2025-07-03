using BotFW_CvSharp_01;
using BotFW_CvSharp_01.CvThings;
using BotFW_CvSharp_01.GlobalStructs;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Reflection;
using Unltimanus.Utils;

namespace Unltimanus.App
{
    public class PortalFinder
    {
        private Dictionary<string, Mat> _templates;
        private string _ho_name = "ho_template";
        private string _map_name = "dune_template";
        private RECT _screenRECT = new RECT(0, 0, 1920, 1080);
        private OpenCvSharp.Point _closestPoint = new(940, 400);
        private Random _rnd;

        private Mat _screen_mask;

        public PortalFinder()
        {
            _templates = Utils.TemplateLoader.LoadPortalTemplates("imgs\\PortalTemplates\\", false);
            _screen_mask = new();
            _rnd = new();
        }

        public bool TryGetPortalClickPoint(MainLoop.Zone zone, out OpenCvSharp.Point point)
        {
            UpdateScreen();

            point = new();
            if (zone == MainLoop.Zone.Map)
            {
                // if zone == ho => need find ho portal
                var hoRes = BotFW.MatchTemplate(_screen_mask, _templates[_ho_name], 0.98f);
                if (hoRes.IsFind)
                {
                    var loc = GetClosestResult(hoRes).Loc;
                    point = RandomUtils.GetRandomPointInRect(new RECT(loc.X, loc.Y, _templates[_ho_name].Width, _templates[_ho_name].Height), _rnd);

                    return true;
                }
            }
            else if (zone == MainLoop.Zone.HO)
            {
                // if zone == ho => need find map portal
                var mapRes = BotFW.MatchTemplate(_screen_mask, _templates[_map_name], 0.98f);
                if (mapRes.IsFind)
                {
                    var loc = GetClosestResult(mapRes).Loc;
                    point = RandomUtils.GetRandomPointInRect(new RECT(loc.X, loc.Y, _templates[_map_name].Width, _templates[_map_name].Height), _rnd);

                    return true;
                }
            }

            return false;
        }

        private ResLoc GetClosestResult(MT_result? res)
        {
            ResLoc closest = res.Res_loc[0];
            if (res.Res_loc.Length == 1)
                return closest;

            var distance = CalculateDistance(res.Res_loc[0].Loc, _closestPoint);

            for (int i = 1; i < res.Res_loc.Length; i++)
            {
                var d = CalculateDistance(res.Res_loc[i].Loc, _closestPoint);
                if (d < distance)
                {
                    distance = d;
                    closest = res.Res_loc[i];
                }
            }
            
            return closest;
        }

        private double CalculateDistance(OpenCvSharp.Point p1, OpenCvSharp.Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void UpdateScreen()
        {
            using var bmp = new Bitmap(_screenRECT.w, _screenRECT.h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BotFW.GetScreen(bmp, _screenRECT, BotFW_CvSharp_01.GameClientThings.WinState.FullScreen);

            using var screen_color = bmp.ToMat();

            // L:100, 11, 104;U:178, 30, 248;E:D:1:3;B:D:3:3
            BotFW.GetMask(screen_color, _screen_mask, new Scalar(100, 11, 104), new Scalar(178, 30, 248), convert_to_hsv: true);
        }

    }
}
