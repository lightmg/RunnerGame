namespace Game.Models
{
    public class GameFieldSize
    {
        public readonly double Width;
        public readonly double Height;

        public GameFieldSize(double height, double width)
        {
            Height = height;
            Width = width;
        }
    }
}