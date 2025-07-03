using BotFW_CvSharp_01.GlobalStructs;
using OpenCvSharp;

namespace Unltimanus.Utils
{
    public static class RandomUtils
    {
        public static Point GetRandomPointInRect(RECT rect, Random rnd)
        {
            var x = rnd.Next(rect.x, rect.x + rect.w);
            var y = rnd.Next(rect.y, rect.y + rect.h);

            return new Point(x, y);
        }

        public static Point GetRandomPointInRect(RECT rect, Point offset, Random rnd)
        {
            var x_w_offset = rect.x + offset.X;
            var y_w_offset = rect.y + offset.Y;
            var w_w_offest = x_w_offset + rect.w - offset.X;
            var h_w_offset = y_w_offset + rect.h - offset.Y;
            if (w_w_offest <= x_w_offset && h_w_offset <= y_w_offset)
                throw new Exception("Invalid Offest!");

            var x = rnd.Next(x_w_offset, w_w_offest);
            var y = rnd.Next(y_w_offset, h_w_offset);

            return new Point(x, y);
        }
    }
}
