namespace Game.Models
{
    /// <summary>
    /// Left bottom corner coordinate of game object
    /// Coordinate grid has (0,0) coordinate in bottom middle point of game field
    /// </summary>
    public class InGamePosition : ICloneable<InGamePosition>
    {
        public double X { get; set; }
        public double Y { get; set; }

        public InGamePosition Clone()
        {
            return new InGamePosition
            {
                X = X,
                Y = Y
            };
        }

        public InGamePosition Add(InGamePosition other) => Add(other.X, other.Y);

        public InGamePosition Add(double x = 0, double y = 0)
        {
            X += x;
            Y += y;
            return this;
        }
    }
}