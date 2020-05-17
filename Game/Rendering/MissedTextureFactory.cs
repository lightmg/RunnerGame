using System.Collections.Generic;
using System.Drawing;
using Game.Models;

namespace Game.Rendering
{
    public class MissedTextureFactory
    {
        private readonly Dictionary<GameObjectSize, Image> texturesCache = new Dictionary<GameObjectSize, Image>();

        public Image Get(GameObjectSize size)
        {
            if (texturesCache.TryGetValue(size, out var image))
                return image;
            image = CreateDefaultTexture(size);
            texturesCache.Add(size, image);
            return image;
        }

        private static Image CreateDefaultTexture(GameObjectSize size)
        {
            var segmentSize = new Size(size.Width / 2, size.Height / 2);

            var bitmap = new Bitmap(size.Width, size.Height);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.Magenta, new Rectangle(new Point(0, 0), segmentSize));
            graphics.FillRectangle(Brushes.Black, new Rectangle(new Point(segmentSize.Width, 0), segmentSize));
            graphics.FillRectangle(Brushes.Magenta, new Rectangle(new Point(segmentSize.Width, segmentSize.Height),
                segmentSize));
            graphics.FillRectangle(Brushes.Black, new Rectangle(new Point(0, segmentSize.Height),
                segmentSize));

            return bitmap;
        }
    }
}