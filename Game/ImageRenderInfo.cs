using System.Drawing;

namespace Game
{
    public class ImageRenderInfo
    {
        private Image image;

        public Image Image
        {
            get => (Image) image.Clone();
            set => image = value;
        }

        public PointF Point { get; set; }
    }
}