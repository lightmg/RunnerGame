using System.Drawing;
using Game.Models;

namespace Game.Helpers
{
    public static class DrawingHelpers
    {
        public static Image CreateSquare(GameObjectParameters parameters, Color color) =>
            CreateSquare(parameters.Size.Width, parameters.Size.Height, color);

        public static Image CreateSquare(int width, int height, Color color)
        {
            var image = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(image);
            graphics.Clear(color);
            return image;
        }

        public static Image AddBorders(this Image image, Color color)
        {
            using var graphics = Graphics.FromImage(image);
            graphics.DrawRectangle(new Pen(color, 3), -1, 0, image.Width, image.Height);
            return image;
        }
    }
}