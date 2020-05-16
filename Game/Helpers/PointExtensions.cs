using System.Drawing;

namespace Game.Helpers
{
    public static class PointExtensions
    {
        public static PointF Add(this PointF point, double X, double Y)
        {
            return new PointF
            {
                X = (float) (point.X + X),
                Y = (float) (point.Y + Y)
            };
        }
    }
}